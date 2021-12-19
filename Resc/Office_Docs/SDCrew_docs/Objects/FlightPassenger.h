//
//  FlightPassenger.h
//  SDCrew
//
//  Created by Adnan hasan on 12/11/19.
//  Copyright © 2021 Satcom Direct, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "YapDatabaseObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface FlightPassenger : YapDatabaseObject
@property (nonatomic, strong, nullable) NSString *flightPassengerId;
@property (nonatomic, strong, nullable) NSString *flightId;
@property (nonatomic, strong, nullable) NSString *postedFlightId;
@property (nonatomic, strong, nullable) NSString *passengerId;
@property (nonatomic, strong, nullable) NSString *customerId;
@property (nonatomic, strong, nullable) NSString *businessCategoryId;
@property (nonatomic, strong, nullable) NSString *passengerStatusId;
@property (nonatomic, strong, nullable) NSString *businessPersonId;
@property (nonatomic, strong, nullable) NSString *personId;
@property (nonatomic, strong, nullable) NSString *phoneNumber;
@property (nonatomic, strong, nullable) NSString *phoneCode;
@property (nonatomic, strong, nullable) NSString *emailAddress;
@property (nonatomic, strong, nullable) NSString *phone;
@property (nonatomic, strong, nullable) NSString *email;
@property (nonatomic, strong, nullable) NSString *passengerFirstName;
@property (nonatomic, strong, nullable) NSString *passengerLastName;
@property (nonatomic) BOOL hasLocalModification;
@property (nonatomic) BOOL needToPost;
@property (nonatomic) BOOL isUpdated;
@property (nonatomic) BOOL isAdded;
-(FlightPassenger *)duplicate;
- (void)didChangeValueForKey:(NSString *)key;
+ (BOOL)automaticallyNotifiesObserversForKey:(NSString *)key;
@end

NS_ASSUME_NONNULL_END
