//
//  RemoteSOViewController.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "RemoteSOViewController.h"
#import "AppDelegate.h"
#import "DEBUG.h"

@interface RemoteSOViewController ()

@end

@implementation RemoteSOViewController

#pragma mark -
#pragma mark  View lifecycle

-(id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        self.title = @"Remote SO";
        self.tabBarItem.image = [UIImage imageNamed:@"remoteso"];
    }
    return self;
}

-(void)viewDidLoad {
    [
     super viewDidLoad];
	
    socket = nil;
    alerts = 100;
    
    clientSO = nil;    
    intSO = 0;
    floatSO = 0.0f;
    
    hostTextField.text = [AppDelegate defaultSharedObjectsURL];
    hostTextField.delegate = self;
	
	netActivity = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
	netActivity.center = CGPointMake(160.0f, 220.0f);
	[self.view addSubview:netActivity];
    
    [DebLog setIsActive:YES];
}

-(void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

-(BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark -
#pragma mark Private Methods 

-(void)showAlert:(NSString *)message {
    UIAlertView *av = [[UIAlertView alloc] initWithTitle:@"Receive" message:message delegate:self cancelButtonTitle:@"Ok" otherButtonTitles:nil];
	alerts++;
	av.tag = alerts;
    [av show];
}

// CONNECTION

-(void)socketConnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = YES;
    noteTextView.hidden = NO;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Disconnect";     
    
    btnConnectSO.hidden = NO;
    btnGetAttributeSO.hidden = NO;
    btnSendMessageSO.hidden = NO;
    btnRemoveAttributeSO.hidden = NO;
    
}

-(void)socketDisconnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = NO;    
    noteTextView.hidden = YES;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Connect";     
    
    btnConnectSO.hidden = YES;
    btnGetAttributeSO.hidden = YES;
    btnSendMessageSO.hidden = YES;
    btnRemoveAttributeSO.hidden = YES;
    
}

-(void)doConnect {				
    
    socket = [[RTMPClient alloc] init:hostTextField.text];
    socket.delegate = self;
    [socket connect];
    
    btnConnect.enabled = NO;     
    
    [netActivity startAnimating];
}

-(void)doDisconnect {	
    
    [socket disconnect];
    socket = nil;
    
    [self socketDisconnected];
}


#pragma mark -
#pragma mark Public Methods 

// ACTIONS

-(IBAction)connectControl:(id)sender {
    (!socket) ? [self doConnect] : [self doDisconnect];
}

// SO

-(IBAction)connectSO:(id)sender {	
    
    if (!clientSO) {
        
        printf("connectSO SEND ----> getSharedObject\n");
        
        // send "getSharedObject (+ connect)"
        NSString *name = [NSString stringWithString:@"SharedBall"];
        clientSO = [socket getSharedObject:name persistent:NO owner:self];
    }
    else 
        if (![clientSO isConnected]) {
            
            printf("connectSO SEND ----> connect\n");
            
            // send "connect"
            [clientSO connect];
        }
        else {
            
            printf("connectSO SEND ----> disconnect\n");
            
            // send "disconnect"
            [clientSO disconnect];
        }
}

-(IBAction)getAttributeSO:(id)sender {	
    
    if (!clientSO || ![clientSO isConnected]) {
        [self showAlert:@"clientSO is absent or disconnected!\n Push 'connectSO' button\n"];
        return;
    }
 	
    NSLog(@"*****************>>>> getAttributeSO: %@ (attributes = %@)", [clientSO getName], [clientSO getAttributeNames]);
    
    intSO += 5;
    floatSO += 5.7f;
    
    // setAttributes
    NSMutableDictionary *dict = [NSMutableDictionary dictionary];    
    [dict setValue:[NSString stringWithFormat:@"itIsString= %d, %g", intSO, floatSO] forKey:[NSString stringWithString:@"stringVal"] ];
    [dict setValue:[NSNumber numberWithInt:intSO] forKey:[NSString stringWithString:@"intVal"] ];
    [dict setValue:[NSNumber numberWithFloat:floatSO] forKey:[NSString stringWithString:@"floatVal"] ];
    [dict setValue:[NSNumber numberWithBool:YES] forKey:[NSString stringWithString:@"boolVal"] ];
    
    [clientSO setAttributes:dict];
}

-(IBAction)removeAttributeSO:(id)sender {	
    
    if (!clientSO || ![clientSO isConnected]) {
        [self showAlert:@"clientSO is absent or disconnected!\n Push 'connectSO' button\n"];
        return;
    }
 	
    NSLog(@"*****************>>>> removeAttributeSO: %@ (attributes = %@)", [clientSO getName], [clientSO getAttributeNames]);
    
    // clear (removeAttributes)
    [clientSO clear];
}

-(IBAction)sendMessageSO:(id)sender {	
    
    if (!clientSO || ![clientSO isConnected]) {
        [self showAlert:@"clientSO is absent or disconnected!\n Push 'connectSO' button\n"];
        return;
    }
 	
    NSLog(@"*****************>>>> sendMessageSO: %@ (attributes = %@)", [clientSO getName], [clientSO getAttributeNames]);
    
    // sendMessageSO
    NSMutableArray *array = [NSMutableArray array];    
    [array addObject:[NSString stringWithString:@"attrString"]];
    [array addObject:[NSNumber numberWithInt:55]];
    [array addObject:[NSNumber numberWithFloat:55.7f]];
    [array addObject:[NSNumber numberWithBool:NO]];
    
    [clientSO sendMessage:@"MEGGAGE_SO" arguments:array];
}

#pragma mark -
#pragma mark UITextFieldDelegate Methods 

-(BOOL)textFieldShouldReturn:(UITextField *)textField {
	[textField resignFirstResponder];
	return YES;
}


#pragma mark -
#pragma mark IRTMPClientDelegate Methods 

-(void)connectedEvent {
    [self socketConnected];
}

-(void)disconnectedEvent {	
    [self doDisconnect];
 	[self showAlert:[NSString stringWithString:@"Disconnected"]];   
}

-(void)connectFailedEvent:(int)code description:(NSString *)description {
    
    [self doDisconnect];
    
    [self showAlert:(code == -1) ? 
     [NSString stringWithFormat:@"Unable to connect to the server. Make sure the hostname/IP address and port number are valid\n"] : 
     [NSString stringWithFormat:@"connectFailedEvent: %@ \n", description]];    
}

-(void)resultReceived:(id <IServiceCall>)call {
   
    NSString *method = [call getServiceMethodName];
    NSArray *args = [call getArguments];
    [self showAlert:[NSString stringWithFormat:@"'%@': arguments = %@\n", method, (args.count)?[args objectAtIndex:0]:@"0"]];
}

#pragma mark -
#pragma mark ISharedObjectListener Methods 

-(void)onSharedObjectConnect:(id <IClientSharedObject>)so {

    if ([so isConnected])
        [btnConnectSO setTitle:@"Disconnect SO" forState:UIControlStateNormal];
	
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectConnect ('%@')\n", [so getName]]];
}

-(void)onSharedObjectDisconnect:(id <IClientSharedObject>)so {

    if (![so isConnected])
        [btnConnectSO setTitle:@"Connect SO" forState:UIControlStateNormal];
	
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectDisconnect ('%@')\n", [so getName]]];
}

-(void)onSharedObjectUpdate:(id <IClientSharedObject>)so withKey:(id)key andValue:(id)value {
        
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectUpdate ('%@') withKey:%@ -> %@\n", [so getName], key, value]];
}

-(void)onSharedObjectUpdate:(id <IClientSharedObject>)so withValues:(id <IAttributeStore>)values {
    
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectUpdate('%@') withValues:%@", [so getName], [values getAttributes]]];
}

-(void)onSharedObjectUpdate:(id <IClientSharedObject>)so withDictionary:(NSDictionary *)values {
        
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectUpdate('%@') withDictionary:%@", [so getName], values]];   
}

-(void)onSharedObjectDelete:(id <IClientSharedObject>)so withKey:(NSString *)key {
    
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectDelete('%@') withKey:%@", [so getName], key]];  
}

-(void)onSharedObjectClear:(id <IClientSharedObject>)so {
    
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectClear('%@')", [so getName]]];    
}

-(void)onSharedObjectSend:(id <IClientSharedObject>)so withMethod:(NSString *)method andParams:(NSArray *)parms {
    
    [self showAlert:[NSString stringWithFormat:@"EVENT: onSharedObjectSend('%@') withMethod:%@ andParams:%@", [so getName], method, parms]];   
}

@end
