//
//  PreFlightObject.h
//  SDCrew
//
//  Created by Arifur Rahman on 22/12/19.
//  Copyright © 2021 Satcom Direct, Inc. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "YapDatabaseObject.h"

#import "FlightPassenger.h"
#import "PilotObject.h"
#import "FboObject.h"
NS_ASSUME_NONNULL_BEGIN

@interface PreFlightObject : YapDatabaseObject


@property (nonatomic, strong, nullable) NSDate *startDateLocal;
@property (nonatomic, strong, nullable) NSDate *stopDateLocal;
@property (nonatomic, strong, nullable) NSDate *startDateUtc;
@property (nonatomic, strong, nullable) NSDate *stopDateUtc;
@property (nonatomic, strong, nullable) NSDate *date;
@property (nonatomic, strong, nullable) NSString *staffEventType;
@property (nonatomic, strong, nullable) NSString *flightId;
@property (nonatomic, strong, nullable) NSString *scheduledAircraftTripId;
@property (nonatomic, strong, nullable) NSString *aircraftEventType;
@property (nonatomic, strong, nullable) NSString *aircraftEventTypeId;
@property (nonatomic, strong, nullable) NSString *airportName;
@property (nonatomic, strong, nullable) NSString *airportIcao;
@property (nonatomic, strong, nullable) NSString *arrivalIcao;
@property (nonatomic, strong, nullable) NSString *departureIcao;
@property (nonatomic, strong, nullable) NSObject *color;
@property (nonatomic, strong, nullable) NSString *secName;
@property (nonatomic, strong, nullable) NSString *event; //Staff, Leg, Aircraft
@property (nonatomic) BOOL tentativeEta;
@property (nonatomic) BOOL tentativeEtd;
@property (nonatomic, strong, nullable) NSString *ete;
@property (nonatomic, strong, nullable) NSString *crewInitials;
@property (nonatomic, strong, nullable) NSString *legNumber;
@property (nonatomic, strong, nullable) NSString *legCount;
@property (nonatomic, strong, nullable) NSString *passengerLegCount;
@property (nonatomic, strong, nullable) NSString *customTripId;
@property (nonatomic, strong, nullable) NSString *customerId;
@property (nonatomic, strong, nullable) NSString *staffEventTypeId;
@property (nonatomic, strong, nullable) NSString *notes;
@property (nonatomic, strong, nonnull) NSArray<FlightPassenger*> *passengers;
@property (nonatomic, strong, nonnull) NSArray<PilotObject*> *crews;
@property (nonatomic, strong, nonnull) NSArray<FboObject*> *fbos;
@property (nonatomic) BOOL isUpdated;

- (void)didChangeValueForKey:(NSString *)key;
+ (BOOL)automaticallyNotifiesObserversForKey:(NSString *)key;
-(PreFlightObject *)duplicate;

@end

NS_ASSUME_NONNULL_END
