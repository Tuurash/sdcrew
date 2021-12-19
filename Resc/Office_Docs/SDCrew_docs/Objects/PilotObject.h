//
//  PilotObject.h
//  SDCrew
//
//  Created by Salman  on 12/3/19.
//  Copyright © 2021 Satcom Direct, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "YapDatabaseObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface PilotObject : YapDatabaseObject


@property (nonatomic, strong, nullable) NSString *crewMemberId;
@property (nonatomic, strong, nullable) NSString *crewMemberTypeId;
@property (nonatomic, strong, nullable) NSString *firstName;
@property (nonatomic, strong, nullable) NSString *flightCrewMemberId;
@property (nonatomic, strong, nullable) NSString *flightId;
@property (nonatomic, strong, nullable) NSString *preFlightId;
@property (nonatomic) BOOL hasLocalModification;
@property (nonatomic) BOOL hold;
@property (nonatomic, strong, nullable) NSString *instTime;
@property (nonatomic) BOOL isSaved;
@property (nonatomic, strong, nullable) NSString *landingsDay;
@property (nonatomic, strong, nullable) NSString *landingsNight;
@property (nonatomic, strong, nullable) NSString *lastName;
@property (nonatomic, strong, nullable) NSString *logbookId;
@property (nonatomic) BOOL newPilot;
@property (nonatomic, strong, nullable) NSString *nightTime;
@property (nonatomic, strong, nullable) NSString *postedFlightId;
@property (nonatomic, strong, nullable) NSString *role;
@property (nonatomic, strong, nullable) NSString *phoneNumber;
@property (nonatomic, strong, nullable) NSString *takeOffDay;
@property (nonatomic, strong, nullable) NSString *takeOffNight;
@property (nonatomic) BOOL track;
@property (nonatomic, strong, nullable) NSObject *updatedKeys;
@property (nonatomic) BOOL isUpdated;
-(PilotObject *)duplicate;
- (void)didChangeValueForKey:(NSString *)key;
+ (BOOL)automaticallyNotifiesObserversForKey:(NSString *)key;
@end

NS_ASSUME_NONNULL_END
