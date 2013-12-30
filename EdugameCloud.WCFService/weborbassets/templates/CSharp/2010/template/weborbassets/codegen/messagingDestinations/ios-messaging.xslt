<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:codegen="urn:cogegen-xslt-lib:xslt"
    xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:template match="/">
    <xsl:param name="projectGuid" select="codegen:getGuid()"/>
    <folder name="weborb-codegen">
      <folder name="MessagingDest">
        <xsl:if test="data/fullCode = 'false'">
          <file path="messagingDestinations\ios\MessagingDest\MessagingClient.h" />
          <file path="messagingDestinations\ios\MessagingDest\MessagingClient.m" />
        </xsl:if>
        <xsl:if test="data/fullCode = 'true'">
          <folder path="messagingDestinations\ios\MessagingDest.xcodeproj" />  
          <folder name="MessagingDest">
            <folder path="messagingDestinations\ios\MessagingDest\en.lproj" />
            <file path="messagingDestinations\ios\MessagingDest\MessagingDest-Info.plist" />
            <file path="messagingDestinations\ios\MessagingDest\MessagingDest-Prefix.pch" />
            <file path="messagingDestinations\ios\MessagingDest\main.m" />
            <file path="messagingDestinations\ios\MessagingDest\MessagingClient.h" />
            <file path="messagingDestinations\ios\MessagingDest\MessagingClient.m" />
            <file path="messagingDestinations\ios\MessagingDest\ViewController.h" />
            <file path="messagingDestinations\ios\MessagingDest\ViewController.m" />
            <file path="messagingDestinations\ios\MessagingDest\AppDelegate.h" />
            <xsl:call-template name="AppDelegate.m.file" /> 
          </folder>
        </xsl:if>
      </folder>
    </folder>
  </xsl:template>
    
  <xsl:template name="AppDelegate.m.file">
      <file name="AppDelegate.m">//
//  AppDelegate.m
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//
          
#import "AppDelegate.h"
          
@implementation AppDelegate
@synthesize window = _window;
          
-(BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    return YES;
}
          
-(void)applicationWillResignActive:(UIApplication *)application {
}
          
-(void)applicationDidEnterBackground:(UIApplication *)application {
}
          
-(void)applicationWillEnterForeground:(UIApplication *)application {
}
          
-(void)applicationDidBecomeActive:(UIApplication *)application {
}
          
-(void)applicationWillTerminate:(UIApplication *)application {
}
          
+(NSString *)defaultURL {
    return @"<xsl:value-of select="data/weborbURL"/>";
}
          
+(NSString *)defaultDestinationName {
    return @"<xsl:value-of select="data/destinationId"/>";
}
          
@end
    </file>
  </xsl:template>
</xsl:stylesheet>
