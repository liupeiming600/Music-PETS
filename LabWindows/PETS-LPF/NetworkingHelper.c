#include <ansi_c.h>
#include "NetworkingHelper.h"
#include "Endianness.h"

NetMessage* createNetMessage( int capacity )
{
	NetMessage* message = (NetMessage*)malloc( sizeof( NetMessage ) );
	message->size = 0;
	message->capacity = capacity;
	message->data = (unsigned char*)malloc( sizeof( unsigned char ) * capacity );
	
	return message;
}

void deleteNetMessage( NetMessage* message )
{
	free( message );
	message = NULL;
}

void writeInt32( NetMessage* message, int val )
{
	int index = message->size;
	int sizeInt = sizeof( int );
	
	if( message->size + sizeInt >= message->capacity )
	{
		int newCapacity = sizeInt + message->capacity * 2;
		unsigned char* newData = (unsigned char*)malloc( newCapacity * sizeof( unsigned char ) );
		memcpy( newData, message->data, message->size );
		
		free( message->data );
		message->data = newData;
	}
	
	message->data[index] = ( val >> 24 ) & 0xFF;
	message->data[index + 1] = ( val >> 16 ) & 0xFF;
	message->data[index + 2] = ( val >> 8 ) & 0xFF;
	message->data[index + 3] = val & 0xFF;
	
	message->size += sizeInt;
}

void writeDouble( NetMessage* message, double val )
{
	int index = message->size;
	int sizeDouble = sizeof( double );
	
	if( message->size + sizeDouble >= message->capacity )
	{
		int newCapacity = sizeDouble + message->capacity * 2;
		unsigned char* newData = (unsigned char*)malloc( newCapacity * sizeof( unsigned char ) );
		memcpy( newData, message->data, message->size );
		
		free( message->data );
		message->data = newData;
	}
	
	DOUBLEUNION_t dblUnion;
	dblUnion.d = val;
	
	for( int i = 0; i < sizeof( double ); i++ )
	{
		// If our machine is little endian, we need to reverse
		// the bytes to turn it into network byte order (big-endian)
		int otherEnd = i;
		if( isLittleEndian() )
		{
			otherEnd = sizeof( double ) - i - 1;
		}
		message->data[index + i] = dblUnion.bytes[otherEnd];
	}
	
	message->size += sizeof( double );
}

void sendUDP( int udpWriterChannel, NetMessage* message, char* dest, int port )
{
	int sendBufSize = ( 4 + message->size ) * sizeof( unsigned char );
	unsigned char* sendBuf = (unsigned char*)malloc( ( 4 + message->size ) * sizeof( unsigned char ) );
	
	// The first 4 bytes of the byte array to be sent
	// will contain the length of the actual data.
	sendBuf[0] = ( message->size >> 24 ) & 0xFF;
	sendBuf[1] = ( message->size >> 16 ) & 0xFF;
	sendBuf[2] = ( message->size >> 8 ) & 0xFF;
	sendBuf[3] = message->size & 0xFF;
	
	// The rest of the byte array will be the actual data.
	memcpy( sendBuf + 4, message->data, message->size );
	
	UDPWrite( udpWriterChannel, port, dest, sendBuf, sendBufSize );
}
