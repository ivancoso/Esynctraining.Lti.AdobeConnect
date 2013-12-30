//
//  MessagingClient.m
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "MessagingClient.h"

@implementation MessagingClient
@synthesize delegate;

-(id)init {	
	if ( (self=[super init]) ) {
        client = nil;
    }
	
	return self;
}

#pragma mark -
#pragma mark Private Methods 

// RESPONDERS

-(void)subscribedHandler:(NSString *)info {
    if ([delegate respondsToSelector:@selector(subscribedHandler:)])
        [delegate subscribedHandler:info];
}

-(void)publishResponseHandler:(id)response {
    if ([delegate respondsToSelector:@selector(publishResponseHandler:)])
        [delegate publishResponseHandler:response];
}

-(void)publishErrorHandler:(Fault *)fault {
    if ([delegate respondsToSelector:@selector(publishErrorHandler:)])
        [delegate publishErrorHandler:fault];
}

-(void)subscribeResponseHandler:(id)response {
    if ([delegate respondsToSelector:@selector(subscribeResponseHandler:)])
        [delegate subscribeResponseHandler:response];
}

-(void)subscribeErrorHandler:(Fault *)fault {
    if ([delegate respondsToSelector:@selector(subscribeErrorHandler:)])
        [delegate subscribeErrorHandler:fault];
}

// ACTIONS

-(void)connect:(NSString *)url destination:(NSString *)destination {	
    
    if (client) 
        [self disconnect];
    
    client = (destination.length) ? 
        [[WeborbClient alloc] initWithUrl:url destination:destination] : [[WeborbClient alloc] initWithUrl:url];
    client.subscribedHandler = [SubscribedHandler responder:self selSubscribedHandler:@selector(subscribedHandler:)];
}

-(void)disconnect {	
    
    if (!client) 
        return;
    
    [client stop];
    client = nil;
}


-(void)publish:(id)object {
    [client publish:object 
          responder:[Responder responder:self 
                      selResponseHandler:@selector(publishResponseHandler:) 
                         selErrorHandler:@selector(publishErrorHandler:)] 
     ];
}

-(void)publish:(id)object headers:(NSDictionary *)headers {
    [client publish:object 
          responder:[Responder responder:self 
                      selResponseHandler:@selector(publishResponseHandler:) 
                         selErrorHandler:@selector(publishErrorHandler:)] 
            headers:headers
     ];
}

-(void)publish:(id)object subtopic:(NSString *)subtopic {
    [client publish:object 
          responder:[Responder responder:self 
                      selResponseHandler:@selector(publishResponseHandler:) 
                         selErrorHandler:@selector(publishErrorHandler:)] 
           subtopic:subtopic 
     ];
}

-(void)publish:(id)object subtopic:(NSString *)subtopic headers:(NSDictionary *)headers {
    [client publish:object 
          responder:[Responder responder:self 
                      selResponseHandler:@selector(publishResponseHandler:) 
                         selErrorHandler:@selector(publishErrorHandler:)] 
           subtopic:subtopic 
            headers:headers
     ];
}

-(void)subscribe {
    [client subscribe:[SubscribeResponder responder:self 
                                 selResponseHandler:@selector(subscribeResponseHandler:) 
                                    selErrorHandler:@selector(subscribeErrorHandler:)] 
     ];
}

-(void)subscribe:(NSString *)subtopic {
    [client subscribe:[SubscribeResponder responder:self 
                                 selResponseHandler:@selector(subscribeResponseHandler:) 
                                    selErrorHandler:@selector(subscribeErrorHandler:)] 
             subtopic:subtopic 
     ];
}

-(void)subscribe:(NSString *)subtopic selector:(NSString *)selector {
    [client subscribe:[SubscribeResponder responder:self 
                                 selResponseHandler:@selector(subscribeResponseHandler:) 
                                    selErrorHandler:@selector(subscribeErrorHandler:)] 
             subtopic:subtopic 
             selector:selector
     ];
}

-(void)unsubscribe {
    [client unsubscribe];
}

-(void)unsubscribe:(NSString *)subtopic {
    [client unsubscribe:subtopic];
}

-(void)unsubscribe:(NSString *)subtopic selector:(NSString *)selector {
    [client unsubscribe:subtopic selector:selector];
}

@end
