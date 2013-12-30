//
//  AppDelegate.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;
@property (strong, nonatomic) UITabBarController *tabBarController;

+(NSString *)defaultHostURL;
+(NSString *)defaultStreamURL;
+(NSString *)defaultStreamName;
+(NSString *)defaultCallbackDemoURL;
+(NSString *)defaultClientInvokeURL;
+(NSString *)defaultSharedObjectsURL;

@end
