#include <stdio.h>
#include <analysis.h>
#include <udpsupp.h>
#include <ansi_c.h>
#include "toolbox.h"
#include <NIDAQmx.h>
#include <cvirte.h>
#include <userint.h>
#include "FingersVoltagesGraphs.h"
#include "VoltageAcquisitionTask.h"
#include "NetworkingHelper.h"

static int panelHandle;

static TaskHandle daqTask;
static unsigned int numChannels;
static int samplesPerChannel = 1000, buffer_size = 100;
static float64 *data;

static unsigned int udpWriterChannel;
static char destIP[20];
static int port = 11000;

static int isSending = 0;

int windowSize = 3;
static double buffer[5][3];
static float64 *mybuffer=NULL;  
static int n = 0;
static int isBufferFilled = 0;

FILE *fp;

// Utility array to handle mapping
// between the 5 fingers and their corresponding
// channel in the PETS hardware.
int fingerMapping[5];

int main( int argc, char* argv[] )
{
	if( InitCVIRTE( 0, argv, 0 ) == 0 )
		return -1; /* out of memory */
	
	if( ( panelHandle = LoadPanel( 0, "FingersVoltagesGraphs.uir", PANEL ) ) < 0 )
		return -1;
	
	CreateUDPChannel( UDP_ANY_LOCAL_PORT, &udpWriterChannel );
	
	CreateVoltageAcquisitionTask( &daqTask );
	DAQmxGetTaskAttribute( daqTask, DAQmx_Task_NumChans, &numChannels );
	
	data = (float64*)malloc( numChannels * samplesPerChannel * sizeof( float64 ) );
	mybuffer = (float64*)malloc( numChannels * buffer_size * sizeof( float64 ) );
	
	// Right now, the PETS hardware is wired as follows:
	// (Assuming left hand)
	// Finger 0 (Pinky): Dev1/ai1
	// Finger 1 (Ring): Dev1/ai3
	// Finger 2 (Middle): Dev/ai0
	// Finger 3 (Index): Dev/ai2
	// Finger 4 (Thumb): Dev/ai4
	fingerMapping[0] = 0;//4;
	fingerMapping[1] = 1;//2;
	fingerMapping[2] = 2;//0;
	fingerMapping[3] = 3;//1;
	fingerMapping[4] = 4;//3;
	
	for(int i=0; i< numChannels * buffer_size ; i++){
		mybuffer[i] = 0;
	}
	
	for( int i = 0; i < 5; i++ )
	{
		for( int j = 0; j < windowSize; j++ )
		{
			buffer[i][j] = 0.0;
		}
	}
	
	DisplayPanel( panelHandle );
	RunUserInterface();
	
	//DAQmxStartTask( daqTask );
	
	DAQmxStopTask( daqTask );										  
	DAQmxClearTask( daqTask );
	
	if( panelHandle > 0 )
		DiscardPanel( panelHandle );
	
	if( udpWriterChannel )
		DisposeUDPChannel( udpWriterChannel );
	
	free( data );
	data = NULL;
	free(mybuffer);
	mybuffer = NULL;
	
	return 0;
}

int CVICALLBACK PanelCallback (int panel, int event, void *callbackData,
							   int eventData1, int eventData2)
{
	switch (event)
	{
		case EVENT_GOT_FOCUS:
			break;
		case EVENT_LOST_FOCUS:
			break;
		case EVENT_CLOSE:
			break;
	}
	return 0;
}

int CVICALLBACK QuitCallback (int panel, int control, int event,
							  void *callbackData, int eventData1, int eventData2)
{
	switch (event)
	{
		case EVENT_COMMIT:
			QuitUserInterface( 0 );
			break;
		case EVENT_RIGHT_CLICK:

			break;
	}
	return 0;
}

int getPanelWaveformFromFinger( unsigned int fingerIndex )
{
	switch( fingerIndex )
	{
		case 0:
			return PANEL_WAVEFORM0;
		case 1:
			return PANEL_WAVEFORM1;
		case 2:
			return PANEL_WAVEFORM2;
		case 3:
			return PANEL_WAVEFORM3;
		case 4:
			return PANEL_WAVEFORM4;
	}
	
	return -1;
}

int CVICALLBACK TimerCallback (int panel, int control, int event,
							   void *callbackData, int eventData1, int eventData2)
{
	// Timer is currently set to 30fps (ticks every 1/30 of a second)
	switch (event)
	{
		case EVENT_TIMER_TICK:
			if( isSending )
			{
				int samplesReadPerChannel = 0;
				DAQmxReadAnalogF64( daqTask, DAQmx_Val_Auto, 1.0, DAQmx_Val_GroupByChannel, data, samplesPerChannel * numChannels, &samplesReadPerChannel, 0 );
				//printf("\nsamples: %d\n",samplesReadPerChannel);
				if( samplesReadPerChannel > 0 )
				{
					//fp = fopen("data.csv", "a");
					NetMessage* message = createNetMessage( 100 );
					
					// Indicate that this message is a finger data message.
					// Just so happens that our identifier for a finger data message is 0.
					writeInt32( message, 0 );
					
					for( int i = 0; i < 5; i++ )
					{
						int channel = fingerMapping[i];
						float64 filtered[buffer_size];
						
						for( int j = 0; j < buffer_size; j++ )
						{
							
							if(samplesReadPerChannel >= buffer_size){
								mybuffer[i * buffer_size + j] = data[(channel + 1) * samplesReadPerChannel - buffer_size + j];
							}else{
								if(j < buffer_size - samplesReadPerChannel){
									mybuffer[i * buffer_size + j] = mybuffer[i * buffer_size + samplesReadPerChannel + j];	
								}else{
									mybuffer[i * buffer_size + j] = data[channel * samplesReadPerChannel + j - (buffer_size - samplesReadPerChannel)];	
								}
							}
							
							filtered[j] = mybuffer[i * buffer_size + j];
						}
						
						Bssl_LPF(filtered, buffer_size, 100, 4, 5, filtered);
						double plotValue = filtered[buffer_size-1];
						
						// Write the finger number
						writeInt32( message, i );
						
						// Write the corresponding voltage for the finger
						PlotStripChartPoint( panelHandle, getPanelWaveformFromFinger( i ), plotValue );
						 
						writeDouble( message, plotValue );
						
						/*
						for(int j = 0; j < samplesReadPerChannel; j++ )
						{	
							if( i == 4 ){
								//fp = fopen("data.csv", "a");
								if(fp == NULL){
									printf("ERROR");
									break;
								}else{
									if( j == samplesReadPerChannel - 1 ) fprintf(fp, "%d,%f,%f\n", j, data[channel * samplesReadPerChannel + j], plotValue);
									else fprintf(fp, "%d,%f,%f\n", j, data[channel * samplesReadPerChannel + j], 0.0);
								}
								//fclose(fp);
							}
						}
						*/
						
					}
					
					// Send the data for all 5 fingers in one UDP packet
					sendUDP( udpWriterChannel, message, destIP, port );
					
					deleteNetMessage( message );
					//fclose(fp);
				}
			}
			
			break;
	}
	return 0;
}

// Function called when the "Start" button was pressed.
int CVICALLBACK StartCallback (int panel, int control, int event,
								   void *callbackData, int eventData1, int eventData2)
{
	switch (event)
	{
		case EVENT_COMMIT:
			if( isSending == 0 )
			{
				isSending = 1;
				
				GetCtrlVal( panel, PANEL_IPADDR, destIP );
				GetCtrlVal( panel, PANEL_PORT, &port );
			}
			
			break;
		case EVENT_RIGHT_CLICK:

			break;
	}
	return 0;
}

// Function called when the "Stop" button is pressed.
int CVICALLBACK StopCallback (int panel, int control, int event,
								  void *callbackData, int eventData1, int eventData2)
{
	switch (event)
	{
		case EVENT_COMMIT:
			isSending = 0;
			break;
		case EVENT_RIGHT_CLICK:

			break;
	}
	return 0;
}
