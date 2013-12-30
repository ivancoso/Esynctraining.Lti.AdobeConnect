//
//  MessagingClient.h
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "WeborbClient.h"
#import "Responder.h"

@protocol IMessagingClientDelegate <NSObject>
-(void)subscribedHandler:(NSString *)info;
-(void)publishResponseHandler:(id)response;
-(void)publishErrorHandler:(Fault *)fault;
-(void)subscribeResponseHandler:(id)response;
-(void)subscribeErrorHandler:(Fault *)fault;
@end


@interface MessagingClient : NSObject {
    
    WeborbClient *client;
    id <IMessagingClientDelegate> __weak delegate;    
}
@property (weak, nonatomic) id <IMessagingClientDelegate> delegate;

-(void)connect:(NSString *)url destination:(NSString *)destination;
-(void)disconnect;
-(void)publish:(id)object;
-(void)publish:(id)object headers:(NSDictionary *)headers;
-(void)publish:(id)object subtopic:(NSString *)subtopic;
-(void)publish:(id)object subtopic:(NSString *)subtopic headers:(NSDictionary *)headers;
-(void)subscribe;
-(void)subscribe:(NSString *)subTopic;
-(void)subscribe:(NSString *)subTopic selector:(NSString *)selector;
-(void)unsubscribe;
-(void)unsubscribe:(NSString *)subTopic;
-(void)unsubscribe:(NSString *)subTopic selector:(NSString *)selector;
@end
