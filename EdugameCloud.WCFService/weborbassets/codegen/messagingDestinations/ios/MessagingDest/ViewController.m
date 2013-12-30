//
//  ViewController.m
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "ViewController.h"
#import "AppDelegate.h"
#import "DEBUG.h"

#define CLIENTID_HEADER_KEY @"WebORBClientId"
#define CLIENTID_DEFAULT @"Anonymous"

@interface ViewController (IMessagingClientDelegate) <IMessagingClientDelegate>
@end

@implementation ViewController

#pragma mark -
#pragma mark - View lifecycle

-(void)viewDidLoad {
    
    [super viewDidLoad];
    
    [DebLog setIsActive:YES];
    
    client = [MessagingClient new];
    client.delegate = self;
    
    alerts = 100;
    
	hostTextField.delegate = self;
    destTextField.delegate = self;
    clientIdTextField.delegate = self;
    chatTextField.delegate = self;
    
    hostTextField.text = [AppDelegate defaultURL];
    destTextField.text = [AppDelegate defaultDestinationName];

}

-(void)viewDidUnload {
    
    [super viewDidUnload];
    
    [client disconnect];
}

-(BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark -
#pragma mark Private Methods 

// ALERTS

-(void)showAlert:(NSString *)message title:(NSString *)title {
    UIAlertView *av = [[UIAlertView alloc] initWithTitle:title message:message delegate:self cancelButtonTitle:@"Ok" otherButtonTitles:nil];
	alerts++;
	av.tag = alerts;
    [av show];
}


#pragma mark -
#pragma mark Public Methods 

// ACTIONS

-(IBAction)doSettings:(id)sender {
    
    hostTextField.hidden = NO;
    destTextField.hidden = NO;
    btnAccept.hidden = NO;
    
    chatToolBar.hidden = YES;
    clientIdLabel.hidden = YES;
    clientIdTextField.hidden = YES;
    chatTextField.hidden = YES;
    chatTextView.hidden = YES;
    
}

-(IBAction)doAccept:(id)sender {	
    
    [client connect:hostTextField.text destination:destTextField.text];
    
    hostTextField.hidden = YES;
    destTextField.hidden = YES;
    btnAccept.hidden = YES;
    
    chatToolBar.hidden = NO;
    clientIdLabel.hidden = NO;
    clientIdTextField.hidden = NO;
    clientIdTextField.text = nil;
    chatTextField.hidden = NO;
    chatTextField.text = nil;
    chatTextView.hidden = NO;
    chatTextView.text = nil;
    
}

-(IBAction)doSubscribe:(id)sender {
    
    [client subscribe];
    
}

-(IBAction)doUnsubscribe:(id)sender {
    
    [client unsubscribe];
    
}

-(void)doSend {
    
    NSString *value = (clientIdTextField.text.length) ? clientIdTextField.text : [NSString stringWithString:CLIENTID_DEFAULT];
    NSDictionary *headers = [NSDictionary dictionaryWithObject:value forKey:[NSString stringWithString:CLIENTID_HEADER_KEY]];
    
    [client publish:chatTextField.text headers:headers];
    
    chatTextField.text = nil;
    
}

#pragma mark -
#pragma mark UITextFieldDelegate Methods 

-(BOOL)textFieldShouldReturn:(UITextField *)textField {
 	
    [textField resignFirstResponder];
   
    if (textField == chatTextField) 
        [self doSend];
    
	return YES;
}

@end


@implementation ViewController (IMessagingClientDelegate) 

#pragma mark -
#pragma mark IMessagingClientDelegate Methods 

-(void)subscribedHandler:(NSString *)info {
    [self showAlert:info title:@"subscribedHandler:"];
}

-(void)publishResponseHandler:(id)response {
}

-(void)publishErrorHandler:(Fault *)fault {
    [self showAlert:fault.detail title:@"publishErrorHandler:"];
}

-(void)subscribeResponseHandler:(id)response {
    SubscribeResponse *message = (SubscribeResponse *)response;
    NSString *clientId = [message.headers objectForKey:CLIENTID_HEADER_KEY];
    if (!clientId) clientId = CLIENTID_DEFAULT;
    chatTextView.text = [NSString stringWithFormat:@"%@ : '%@'\n%@", clientId, message.response, chatTextView.text];
}

-(void)subscribeErrorHandler:(Fault *)fault {
    [self showAlert:fault.detail title:@"subscribeErrorHandler:"];
}

@end

