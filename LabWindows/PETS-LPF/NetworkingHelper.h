#ifndef NETWORKINGHELPER_INCLUDE
#define NETWORKINGHELPER_INCLUDE

/*
 * The NetworkingHelper header provides utility structs and functions
 * for network related tasks, e.g. struct for consolidating all data into
 * one big chunk before sending them over the network, and function for
 * sending the data via UDP.
 */

#include <udpsupp.h>

// Utility struct for consolidating all data that needs to be sent over the network
// into one big chunk.
typedef struct
{
	unsigned int size;
	unsigned int capacity;
	unsigned char* data;
} NetMessage;

// Convenient way of converting from a double value to a byte array
// for sending over the network.
// Reference: http://www.cplusplus.com/forum/beginner/85527/
typedef union
{
	double d;
	unsigned char bytes[sizeof( double )];
} DOUBLEUNION_t;

NetMessage* createNetMessage( int capacity );
void deleteNetMessage( NetMessage* message );

// Append an integer value to the NetMessage struct
void writeInt32( NetMessage* message, int val );

// Append a double value to the NetMessage struct
void writeDouble( NetMessage* message, double val );

// Send the given NetMessage to the specified ip address and port
void sendUDP( int udpWriterChannel, NetMessage* message, char* dest, int port );

#endif // NETWORKINGHELPER_INCLUDE
