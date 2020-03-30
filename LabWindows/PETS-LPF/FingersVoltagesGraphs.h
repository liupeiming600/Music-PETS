/**************************************************************************/
/* LabWindows/CVI User Interface Resource (UIR) Include File              */
/*                                                                        */
/* WARNING: Do not add to, delete from, or otherwise modify the contents  */
/*          of this include file.                                         */
/**************************************************************************/

#include <userint.h>

#ifdef __cplusplus
    extern "C" {
#endif

     /* Panels and Controls: */

#define  PANEL                            1       /* callback function: PanelCallback */
#define  PANEL_QUITBUTTON                 2       /* control type: command, callback function: QuitCallback */
#define  PANEL_TIMER                      3       /* control type: timer, callback function: TimerCallback */
#define  PANEL_IPADDR                     4       /* control type: string, callback function: (none) */
#define  PANEL_START                      5       /* control type: command, callback function: StartCallback */
#define  PANEL_STOP                       6       /* control type: command, callback function: StopCallback */
#define  PANEL_WAVEFORM0                  7       /* control type: strip, callback function: (none) */
#define  PANEL_WAVEFORM1                  8       /* control type: strip, callback function: (none) */
#define  PANEL_WAVEFORM2                  9       /* control type: strip, callback function: (none) */
#define  PANEL_WAVEFORM3                  10      /* control type: strip, callback function: (none) */
#define  PANEL_PORT                       11      /* control type: numeric, callback function: (none) */
#define  PANEL_WAVEFORM4                  12      /* control type: strip, callback function: (none) */


     /* Control Arrays: */

          /* (no control arrays in the resource file) */


     /* Menu Bars, Menus, and Menu Items: */

          /* (no menu bars in the resource file) */


     /* Callback Prototypes: */

int  CVICALLBACK PanelCallback(int panel, int event, void *callbackData, int eventData1, int eventData2);
int  CVICALLBACK QuitCallback(int panel, int control, int event, void *callbackData, int eventData1, int eventData2);
int  CVICALLBACK StartCallback(int panel, int control, int event, void *callbackData, int eventData1, int eventData2);
int  CVICALLBACK StopCallback(int panel, int control, int event, void *callbackData, int eventData1, int eventData2);
int  CVICALLBACK TimerCallback(int panel, int control, int event, void *callbackData, int eventData1, int eventData2);


#ifdef __cplusplus
    }
#endif
