//
//  VideoPlayerViewController.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "VideoPlayerViewController.h"
#import "AppDelegate.h"
#import "VideoPlayer.h"
#import "DEBUG.h"

@interface VideoPlayerViewController ()

@end

@implementation VideoPlayerViewController

#pragma mark -
#pragma mark  View lifecycle

-(id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        self.title = @"Player";
        self.tabBarItem.image = [UIImage imageNamed:@"player"];
    }
    return self;
}

-(void)viewDidLoad {
    
    [super viewDidLoad];
    
    hostTextField.text = [AppDelegate defaultStreamURL];
    hostTextField.delegate = self;
    
    streamTextField.text = [AppDelegate defaultStreamName];
	streamTextField.delegate = self;
    
    //[DebLog setIsActive:YES];
    
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
    UIAlertView *av = [[UIAlertView alloc] initWithTitle:@"Receive" message:message delegate:self 
                                       cancelButtonTitle:@"Ok" otherButtonTitles:nil];
    [av show];
}

-(void)doConnect {
    
    FramesPlayer *_player = [[FramesPlayer alloc] initWithView:previewView];
    _player.orientation = UIImageOrientationRight;
    
    player = [[MediaStreamPlayer alloc] init:hostTextField.text];
    player.delegate = self;
    player.player = _player;
    player.isSynchronization = YES;
    [player stream:streamTextField.text];    
    
    btnConnect.title = @"Disconnect"; 
}

-(void)doDisconnect {
    
    [player disconnect];
    player = nil;
    
    btnConnect.title = @"Connect";
    btnPlay.title = @"Start";
    btnPlay.enabled = NO;
    
    hostTextField.hidden = NO;
    streamTextField.hidden = NO;
    
    previewView.hidden = YES;
    
}

#pragma mark -
#pragma mark Public Methods 

// ACTIONS

-(IBAction)connectControl:(id)sender {
    
    (!player) ? [self doConnect] : [self doDisconnect];
    
}

-(IBAction)playControl:(id)sender; {
    
    (player.state != STREAM_PLAYING) ? [player start] : [player pause];
    
}

#pragma mark -
#pragma mark UITextFieldDelegate Methods 

-(BOOL)textFieldShouldReturn:(UITextField *)textField {
	[textField resignFirstResponder];
	return YES;
}

#pragma mark -
#pragma mark IMediaStreamEvent Methods 

-(void)stateChanged:(MediaStreamState)state description:(NSString *)description {
    
    switch (state) {
            
        case CONN_DISCONNECTED: {
            
            [self doDisconnect];
            [self showAlert:[NSString stringWithString:description]];   
            
            break;
        }
            
        case STREAM_CREATED: {
            
            [player start];
            
            hostTextField.hidden = YES;
            streamTextField.hidden = YES;
            previewView.hidden = NO;
            
            btnPlay.enabled = YES;
            
            break;
            
        }
            
        case STREAM_PAUSED: {
            
            btnPlay.title = @"Start";
            
            break;
        }
            
        case STREAM_PLAYING: {
            
            if ([description isEqualToString:@"NetStream.Play.StreamNotFound"]) {
                
                [player stop];
                [self showAlert:[NSString stringWithString:description]];   
                
                break;
            }
            
            btnPlay.title = @"Pause";
            
            break;
        }
            
        default:
            break;
    }
}

-(void)connectFailed:(int)code description:(NSString *)description {
    
    [self doDisconnect];
    
    [self showAlert:(code == -1) ? 
     [NSString stringWithFormat:@"Unable to connect to the server. Make sure the hostname/IP address and port number are valid\n"] : 
     [NSString stringWithFormat:@"connectFailedEvent: %@ \n", description]];    
}

@end
