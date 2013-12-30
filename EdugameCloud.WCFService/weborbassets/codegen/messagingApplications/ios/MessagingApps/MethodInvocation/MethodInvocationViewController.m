//
//  MethodInvocationViewController.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "MethodInvocationViewController.h"
#import "AppDelegate.h"
#import "DEBUG.h"

#define STATUS_PENDING 0x01
#define HELP_HTML @"<html><head><style type='text/css'>body {font-family: Helvetica; font-size: 50px;}</style></head><body><p>INSTRUCTIONS: This example requires special code on the server-side which will push data into the client code by invoking client-side functions. In order to deploy the server-side code, make sure that your application adapter either is or extends from Weborb.Examples.RTMPDemo.AppHandler. See the <a href='http://www.themidnightcoders.com/rtmpdemosetup'>product documentation</a> for additional instructions.</p></body></html>"

@interface MethodInvocationViewController ()

@end

@implementation MethodInvocationViewController

#pragma mark -
#pragma mark  View lifecycle

-(id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        self.title = @"Invoke";
        self.tabBarItem.image = [UIImage imageNamed:@"invoke"];
    }
    return self;
}

-(void)viewDidLoad {
    [super viewDidLoad];
	
    socket = nil;
    alerts = 100;
    
    hostTextField.text = [AppDelegate defaultClientInvokeURL];
    hostTextField.delegate = self;
    
	helpWebView.delegate = self;
	helpWebView.scalesPageToFit = YES;
	helpWebView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
	
	netActivity = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
	netActivity.center = CGPointMake(155.0f, 120.0f);
	[helpWebView addSubview:netActivity];
    
    [helpWebView loadHTMLString:HELP_HTML baseURL:NULL];
    
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

-(void)socketConnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = YES;
    helpWebView.hidden = YES;
    noteTextView.hidden = NO;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Disconnect";     
    
    btnEchoInt.hidden = NO;
    btnEchoFloat.hidden = NO;
    btnEchoString.hidden = NO;
    btnEchoStringArray.hidden = NO;
    
}

-(void)socketDisconnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = NO;    
    helpWebView.hidden = NO;
    noteTextView.hidden = YES;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Connect";     
    
    btnEchoInt.hidden = YES;
    btnEchoFloat.hidden = YES;
    btnEchoString.hidden = YES;
    btnEchoStringArray.hidden = YES;
    
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

// CALLBACKS

-(void)onEchoInt:(id)result {
    [self showAlert:[NSString stringWithFormat:@"onEchoInt = %@\n", result]];
}

-(void)onEchoFloat:(id)result {
    [self showAlert:[NSString stringWithFormat:@"onEchoFloat = %@\n", result]];
}

-(void)onEchoString:(id)result {
    [self showAlert:[NSString stringWithFormat:@"onEchoString = %@\n", result]];
}

-(void)onEchoStringArray:(id)result {
    [self showAlert:[NSString stringWithFormat:@"onEchoStringArray = %@\n", result]];
}

#pragma mark -
#pragma mark Public Methods 

// ACTIONS

-(IBAction)connectControl:(id)sender {
    (!socket) ? [self doConnect] : [self doDisconnect];
}

// INVOKES

-(IBAction)echoInt:(id)sender {	
	
	printf(" SEND ----> echoInt\n");
	
	// set call parameters
	NSMutableArray *args = [NSMutableArray array];
	NSString *method = [NSString stringWithString:@"echoInt"];
	[args addObject:[NSNumber numberWithInt:12]];	
	// send invoke
	[socket invoke:method withArgs:args responder:[AsynCall call:self method:@selector(onEchoInt:)]];
}

-(IBAction)echoFloat:(id)sender {	
	
	printf(" SEND ----> echoFloat\n");
	
	NSMutableArray *args = [NSMutableArray array];
	// set call parameters
	NSString *method = [NSString stringWithString:@"echoFloat"];
	[args addObject:[NSNumber numberWithDouble:17.5f]];
	// send invoke
	[socket invoke:method withArgs:args responder:[AsynCall call:self method:@selector(onEchoFloat:)]];
}

-(IBAction)echoString:(id)sender {	
	
	printf(" SEND ----> echoString\n");
	
	NSMutableArray *args = [NSMutableArray array];
	// set call parameters
	NSString *method = [NSString stringWithString:@"echoString"];
	//[args addObject:[NSString stringWithString:@"Hello, WebORB!"]];
	[args addObject:[NSString stringWithString:@"Привет, ВебОРБ!"]];
	// send invoke
	[socket invoke:method withArgs:args responder:[AsynCall call:self method:@selector(onEchoString:)]];
}

-(IBAction)echoStringArray:(id)sender {	
	
	printf(" SEND ----> echoStringArray\n");
	
	NSMutableArray *args = [NSMutableArray array];
	// set call parameters
	NSString *method = [NSString stringWithString:@"echoStringArray"];
	NSMutableArray *param1 = [NSMutableArray array];
	[param1 addObject:[NSString stringWithString:@"FIRST"]];
	[param1 addObject:[NSString stringWithString:@"SECOND"]];
	[param1 addObject:[NSString stringWithString:@"THIRD"]];
	[args addObject:param1];
	// send invoke
	[socket invoke:method withArgs:args responder:[AsynCall call:self method:@selector(onEchoStringArray:)]];
}

#pragma mark -
#pragma mark UITextFieldDelegate Methods 

-(BOOL)textFieldShouldReturn:(UITextField *)textField {
	[textField resignFirstResponder];
	return YES;
}

#pragma mark -
#pragma mark UIWebView Delegate Methods 

-(void)webViewDidStartLoad:(UIWebView *)webView {
	[netActivity startAnimating];
}

-(void)webViewDidFinishLoad:(UIWebView *)webView {
	[netActivity stopAnimating];
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
    
    int status = [call getStatus];
    if (status != STATUS_PENDING) // this call is not a server response
        return;
    
    NSString *method = [call getServiceMethodName];
    NSArray *args = [call getArguments];
    int invokeId = [call getInvokeId];
    id result = (args.count) ? [args objectAtIndex:0] : nil;
    
    NSLog(@" $$$$$$ <IRTMPClientDelegate>> resultReceived <---- status=%d, invokeID=%d, method='%@' arguments=%@\n", status, invokeId, method, result);
    
    [self showAlert:[NSString stringWithFormat:@"'%@': arguments = %@\n", method, result]];    
}

@end
