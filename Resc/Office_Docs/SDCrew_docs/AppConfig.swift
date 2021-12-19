//
//  AppConfig.swift
//  SDCrew
//
//  Created by Asif on 8/19/19.
//  Copyright © 2021 Satcom Direct, Inc. All rights reserved.
//

import UIKit

class AppConfig: NSObject {
    
    struct UserdefaultKeys {
        static let LOGGED_IN_STATUS_KEY = "loginStatus"
        static let AUTH0_SERVICE = "service_auth"
        
        static let REFRESH_TOKEN = "refresh_token_auth"
        static let DEPLOY = "release_option"
        static let POST_FLIGHT_ACCESS = "postflight_permission"
        static let PRE_FLIGHT_ACCESS = "preflight_permission"
        static let CUSTOMER_ID = "customer_id"
        static let POST_STATUS = "post_status"
        static let PRE_FLIGHT_REFRESH_UPDATE_TIME = "pre_flight_refresh_update_time"
        static let POST_FLIGHT_REFRESH_UPDATE_TIME = "post_flight_refresh_update_time"
        static let AIRPORT_REFRESH_UPDATE_TIME = "airport_refresh_update_time"
        static let TIME_ZONE = "time_zone"
        static let POSTFLIGHT_SCROLL_SUB_MENUS = "Postflight Scroll Sub Menus"
        static let EVENT_DAYS = "Event Days Only"
        static let AUTOMATIC_AIRPORT_UPDATES = "Automatic Airport Updates"
        static let OOOI_VISIBILITY = "OOOI"
        static let CREW_VISIBILITY = "Crew"
        static let FUEL_VISIBILITY = "Fuel"
        static let NOTES_VISIBILITY = "Notes"
        static let PASSENGERS_VISIBILITY = "Passengers"
        static let ADDITIONAL_DETAILS_VISIBILITY = "ADDITIONAL DETAILS"
        static let EXPENSES_VISIBILITY = "Expenses"
        static let DOCUMENTS_VISIBILITY = "DOCUMENTS"
        static let DE_ANTI_ICE_VISIBILITY = "De/Anti-Ice"
        static let APU_CUSTOM_VISIBILITY = "APU/Custom Components"
        static let OILS_FLUIDS_VISIBILITY = "Oils & Fluids"
        static let SQUAWKS_DISCREPENCIES_VISIBILITY = "Squawks & Discrepancies"
        static let DUTY_TIME_VISIBILITY = "Duty Time"
        static let CHECKLIST_VISIBILITY = "Checklist"
        static let PREFLIGHT_SCROLL_SUB_MENUS = "Preflight Scroll Sub Menus"
        // Settings options
        static let REFRESH_RATE = "app_refresh_rate_option"
        static let THEME = "app_theme_color_option"
        static let PAST_EVENTS = "app_past_events_option"
        static let FUTURE_EVENTS = "app_future_events_option"
        static let PAST_FLIGHTS = "app_past_flights_option"
        static let POSTFLIGHT_VISIBILITIES = "app_refresh_rate_option"
        static let APP_NOTIFICATION = "app_notifications"
        static let SHOW_MY_FLIGHTS_EVENTS_ONLY = "show_my_flights_events_only"
        //Profile Options
        static let USER_NAME = "USER_NAME"
        static let COMPANY_NAME = "COMPANY_NAME"
        static let EMAIL = "EMAIL"
        
        static let NOTIFICATION_INSTALLATION_ID = "NOTIFICATION_INSTALLATION_ID"
        static let INSTALLATION_ID_USER_NAME = "INSTALLATION_ID_USER_NAME"
        static let EXP_TIME = "EXP_TIME"
        //static let AUTH_TIME = "AUTH_TIME"
        //static let DEVICE_TOKEN = "DEVICE_TOKEN"
        static let APP_VERSION = "APP_VERSION"
        static let IS_SYNCHING = "IS_SYNCHING"
    }
    enum DEPLOYMENT:Int {
        case PRODUCTION = 1
        case TEST
        case DEV
    }
    enum THEME:Int {
        case DARK = 1
        case LIGHT
        case DARKNIGHT
    }
    
    struct Auth0RequestParams {
        static var IDENTIFIER_URL = "https://identity.satcomdirect.com"
        static let CLIENT_ID = "CrewMobileClient"
        static let REDIRECT_URI = "sdcrew://callback"
        static let STATE_KEY = "state"
        static let RESPONSE_TYPE = "code id_token"
        static let SCOPES = ["openid", "profile", "offline_access", "sdportalapi"]
        static let POST_LOGOUT_REDIRECT_URI = "sdcrew://logout"
        static let TIME_INTERVAL_REFRESH_TOKEN = 3500.0
        
        static func Auth0RequestParams_Update() -> Void{
            
            if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.PRODUCTION.rawValue){
                AppConfig.Auth0RequestParams.IDENTIFIER_URL = "https://identity.satcomdirect.com"
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.TEST.rawValue){
                AppConfig.Auth0RequestParams.IDENTIFIER_URL = "https://identity.satcomtest.com"
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.DEV.rawValue){
                AppConfig.Auth0RequestParams.IDENTIFIER_URL = "https://identity.satcomdev.com"
            }
        }
        
    }
    struct RequestAPIS {
        static var POST_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/postflight/"
        static var MOBILE_POST_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/"
        static var PRE_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/preflight/"
        static var PROFILE_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/profile/"
        static var PRE_FLIGHT_PROFILE_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/preflight/"
        // Fetch data api's
        static let FETCH_FLIGHT_LEG_LIST = "api/MobileFlightList/List"
        static let FETCH_TAIL_NUMBERS = "api/AircraftProfile/AircraftProfileDtos"
        static let FETCH_APPROACH_TYPES = "api/Reference/approachType"
        static let FETCH_EXPENSE_TYPES = "api/Reference/expenseCategory"
        static let FETCH_CREW_MEMBER_TYPES = "api/Reference/crewMemberType"
        static let FETCH_CREW_MEMBERS = "api/scheduling/crewmembers"
        static let LOGBOOK_DOCUMENT = "api/LogBookDocument"
        static let FETCH_DEICE_MIX_RATIO_TYPES = "api/Reference/mixRatioType?activeOnly=false&includeSystem=false"
        static let FETCH_DEICE_TYPES = "api/Reference/deiceType?activeOnly=false&includeSystem=false"
        static let FETCH_STAFF_EVENT_TYPES = "api/StaffEvent/StaffEventTypes" //Staff events for preflight,[events in ui]
        static let FETCH_FLIGHT_EXPENSES = "api/MobilePostFlightExpense/Filter"
        static let FETCH_FLIGHT_EXPENSE_IMAGE = "api/Documents"
        static let FETCH_AIRCRAFT_PROFILE = "api/AircraftProfile"
        static let FETCH_POSTED_FLIGHT_COMPONENT = "api/MobilePostFlightComponent/Filter"
        static let FETCH_FLIGHT_COMPONENT = "api/MobilePostFlightComponent/AttachedComponents"
        static let FETCH_DUTY_TIME = "api/MobileCrewMemberMetricDate/ByDateRange"
        static let FETCH_PASSENGERS_STATUS_TYPE = "api/Reference/passengerStatus"
        static let FETCH_SQUAWKS_ATA_CODE = "api/Reference/squawkAtaCode"
        static let FETCH_SQUAWKS_CATAGORY = "api/Reference/squawkCategory"
        static let FETCH_SQUAWKS_TYPE = "api/Reference/squawkType"
        static let FETCH_SQUAWKS_STATUS = "api/Reference/squawkStatus"
        static let FETCH_SQUAWKS_DISCREPANCIES = "api/PostedFlightSquawk/Filter"
        static let FETCH_DOCUMENT_TYPES = "api/Reference/DocumentType"
        static let FETCH_PAYMENT_TYPES = "api/Reference/paymentType"
        static let FETCH_DEPARTMENTS = "api/Reference/department"
        static let FETCH_QUANTITY_TYPES = "api/Reference/quantityType"
        static let FETCH_BUSINESS_CATAGORY = "api/Reference/businessCategory"
        static let FETCH_DELAY_TYPE = "api/Reference/delayType"
        static let FETCH_FBO = "api/Airport/"
        static let FETCH_CHECKLIST = "api/ScheduledAircraftTrip/GetAppliedChecklists"
        static let FETCH_CHECKLIST_TYPES = "api/Reference/ChecklistType"
        static let FETCH_PROFILE_IMAGE_INFO = "api/Images"
        static let FETCH_PREFLIGHT_NOTES = "api/CalendarNote/GetCalendarNotes"
        
        // POST/PUT data api's
        static let POST_FLIGHT_CREW_MEMBER = "api/MobilePostedFlightCrew"
        static let POST_FLIGHT_OOOI = "api/MobilePostFlight"
        static let POST_FLIGHT_FUEL = "api/MobilePostFlightFuel"
        static let POST_FLIGHT_ADDITIONAL_DETAIL = "api/MobilePostFlightAdditionalDetail"
        static let POST_FLIGHT_DeIce = "api/MobilePostedFlightDeIce"
        static let POST_FLIGHT_PASSENGER = "api/MobilePostFlightPassenger"
        static let POST_FLIGHT_FLUID = "api/MobilePostedFlightFluid"
        static let PRE_FLIGHT_AIRCRAFT_EVENT = "api/MobileCalendar/GetCalendarDaysByDateRange" //"api/MobileCalendar/AircraftCalendar"
        static let PRE_FLIGHT_STUFF_EVENT = "api/MobileCalendar/StaffEvents"
        static let POST_FLIGHT_EXPENSE = "api/MobilePostFlightExpense"
        static let POST_FLIGHT_EXPENSE_IMAGE = "api/FileStorage"
        static let POST_FLIGHT_COMPONENT = "api/MobilePostFlightComponent"
        static let POST_FLIGHT_AIRPORT_LIST = "api/MobilePostFlightAirport"
        static let POST_DUTY_TIME = "api/MobileCrewMemberMetricDate/Save"
        static let POST_SQUAWKS_DISCREPANCIES = "api/PostedFlightSquawk"
        static let POST_FLIGHT_DELETE_MANAGED_DOCUMENT = "api/Documents/delete"
        static let POST_PREFLIGHT_CHECKED_CHECKLIST = "api/ScheduledAircraftTrip/SaveCheckedData"
        static let POST_PREFLIGHT_DELETE_CHECKLIST = "api/ScheduledAircraftTrip/DeleteCheckedData"
        static let POST_POSTFLIGHT_CHECKED_CHECKLIST = "api/PostedFlight/SaveCheckedData"
        static let POST_POSTFLIGHT_DELETE_CHECKLIST = "api/PostedFlight/DeleteCheckedData"
        static let VIEW_PREFLIGHT_ITINERARY = "api/Scheduling/Itinerary/GetItinerary"
        static let POST_FLIGHT_ESIGN = "api/PostedFlight/Sign"
        static let UPDATE_PASSENGER = "api/MobileScheduling/UpdatePassengerStatus"
        static let DELETE_PASSENGER = "api/MobileScheduling/DeletePassengers"
        static let SCHEDULE_FLIGHT_PASSENGER = "api/MobileScheduling/GetScheduledFlightPassengers"
        // Notification API url
        static let CREATE_OR_UPDATE_INSTALLATION = "api/AzureNotificationHub/CreateOrUpdateInstallation"
        
        static func RequestAPIS_Update() -> Void{
            
            if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.PRODUCTION.rawValue){
                AppConfig.RequestAPIS.POST_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/postflight/"
                AppConfig.RequestAPIS.MOBILE_POST_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/"
                AppConfig.RequestAPIS.PRE_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/preflight/"
                AppConfig.RequestAPIS.PROFILE_FLIGHT_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/profile/"
                AppConfig.RequestAPIS.PRE_FLIGHT_PROFILE_REQUEST_URL = "https://sd-profile-api.satcomdirect.com/preflight/"
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.TEST.rawValue){
                AppConfig.RequestAPIS.POST_FLIGHT_REQUEST_URL = "https://profile-api.satcomtest.com/postflight/"
                AppConfig.RequestAPIS.MOBILE_POST_FLIGHT_REQUEST_URL = "https://profile-api.satcomtest.com/"
                AppConfig.RequestAPIS.PRE_FLIGHT_REQUEST_URL = "https://profile-api.satcomtest.com/preflight/"
                AppConfig.RequestAPIS.PROFILE_FLIGHT_REQUEST_URL = "https://profile-api.satcomtest.com/profile/"
                AppConfig.RequestAPIS.PRE_FLIGHT_PROFILE_REQUEST_URL = "https://profile-api.satcomtest.com/preflight/"
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.DEPLOY) == AppConfig.DEPLOYMENT.DEV.rawValue){
                AppConfig.RequestAPIS.POST_FLIGHT_REQUEST_URL = "https://profile-api.satcomdev.com/postflight/"
                AppConfig.RequestAPIS.MOBILE_POST_FLIGHT_REQUEST_URL = "https://profile-api.satcomdev.com/"
                AppConfig.RequestAPIS.PRE_FLIGHT_REQUEST_URL = "https://profile-api.satcomdev.com/preflight/"
                AppConfig.RequestAPIS.PROFILE_FLIGHT_REQUEST_URL = "https://profile-api.satcomdev.com/profile/"
                AppConfig.RequestAPIS.PRE_FLIGHT_PROFILE_REQUEST_URL = "https://profile-api.satcomdev.com/preflight/"
            }
            DataManager.shared.loadAirports()
        }
    }
    
    
    
    struct AppColor {
        static var PostflightDetailBackgroungColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
        static var PostflightDetailCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var PostflightDetailContentLabelColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1) //Text Color
        static var PostflightWarningContentLabelColor = #colorLiteral(red: 1.0, green: 0.69411765, blue: 0.23529412, alpha: 1) //Text Color
        static var PostflightDetailDataLabelColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)    //Text Color
        static var PostflightDetailCollapseIconColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var PostflightDetailTitleTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
        static var PostflightDetailBorderColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var PostflightDetailAddButtonBorderColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var PostflightDetailAddButtonTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var PostflightDetailcellBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
        static var PostflightDetailDeleteButtonBackgroundColor = #colorLiteral(red: 0.06666667, green: 0.19215686, blue: 0.41960784, alpha: 1)
        static var PostflightDetailNotesBackgroundColor = #colorLiteral(red: 0.06666667, green: 0.19215686, blue: 0.41960784, alpha: 1)
        static var PostflightDetailSegmentBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
        static var PostflightDetailSelectedSegmentColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var PostflightDetailLogbookColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var PostflightDetailPassengerStatusColor = #colorLiteral(red: 0.6642242074, green: 0.6642400622, blue: 0.6642315388, alpha: 1)
        static var PostflightDetailCrewCellImageColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
        static var PostflightDetailExpenseBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var PostflightDetailCellCardBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
        static var postflightPersonImageColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        
        static var ThumbColor = #colorLiteral(red: 0.331592232, green: 0.6428635716, blue: 0.7931218743, alpha: 1)
        static var CustomSegmentColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var FlightFilterViewBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        
        static var checklistViewBackgroundColor = #colorLiteral(red: 0.09476391226, green: 0.2012417018, blue: 0.2967114449, alpha: 1)
        static var checklistCellColor = #colorLiteral(red: 0.08076592535, green: 0.1359926462, blue: 0.1905433238, alpha: 1)
        
        static var PostflightDetailSwitchThumbColor = #colorLiteral(red: 0.331592232, green: 0.6428635716, blue: 0.7931218743, alpha: 1)
        static var PostflightDetailSwitchOffColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
        static var PostflightDetailSwitchOnColor = #colorLiteral(red: 0.331592232, green: 0.6428635716, blue: 0.7931218743, alpha: 1)
        static var PostflightDetailSwitchBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
        static var CustomNavTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        static var PostflightCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var PostflightNavColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var PostflightBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
        static var PostflightCellContentTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
        static var PostflightCellDataTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        static var PostflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
        static var PostflightCellUTCTextColor = #colorLiteral(red: 0.4806716442, green: 0.564641118, blue: 0.6149906516, alpha: 1)
        static var PostflightCellCardBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
        static var PostflightPilotChecklistColor = #colorLiteral(red: 0, green: 0.9098039216, blue: 0.1450980392, alpha: 1)
        static var PostflightSchedulingChecklistColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
        static var PostflightMaintenanceChecklistColor = #colorLiteral(red: 0.9647058824, green: 0.3254901961, blue: 0.3254901961, alpha: 1)
        static var preflightCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var preflightNavColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var preflightBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
        static var preflightCellContentTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
        static var preflightCellDataTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        static var preflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
        static var preflightCellUTCTextColor = #colorLiteral(red: 0.4806716442, green: 0.564641118, blue: 0.6149906516, alpha: 1)
        static var preflightCellMediumHeaderTextColor = #colorLiteral(red: 0.2065613568, green: 0.627014339, blue: 0.8508805633, alpha: 1)
        static var preflightCellSmallHeaderTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var preflightCellPhoneNumberTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        
        static var PostflightExpenseCellBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
        
        static var popupViewBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var popupViewTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        static var popupViewStarColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
        
        static var filtersBackgroungColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var filtersTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
        
        static var searchViewBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
        static var searchViewPlaceHolderColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
        
        static var statusbarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        
        static var refreshControlColor = UIColor.gray
        
        static var tabBarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var tabBarTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var selectedTabTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var tabBarSelectedItemTintColor = #colorLiteral(red: 0.2065613568, green: 0.627014339, blue: 0.8508805633, alpha: 1)
        static var tabBarUnselectedItemTintColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        
        static var saveImage = UIImage(named: "saveIcon")
        static var activityViewStyle = UIActivityIndicatorView(style: .gray)
        
        static var flightDetailsMenuBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
        static var flightDetailsMenuColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var flightDetailsSelectedMenuColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
        static var flightDetailsMenuTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var flightDetailsSelectedMenuTextColor = #colorLiteral(red: 0.9999960065, green: 1, blue: 1, alpha: 1)
        
        static var settingsNavBarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
        static var settingsPageBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
        static var settingsHeaderTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
        static var settingsSmallHeaderTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
        static var settingsTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
        static var emailImage = UIImage(named: "emailBlack")
        
        static var sideMenuTitleColor = #colorLiteral(red: 0.6666666667, green: 0.6705882353, blue: 0.7058823529, alpha: 1)
        static var sideMenuOptionTitleColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
        static var sideMenuSupportCellColor = #colorLiteral(red: 0.1098039216, green: 0.1647058824, blue: 0.2705882353, alpha: 1)
        static var sideMenuBackgroundColor = #colorLiteral(red: 0.09803921569, green: 0.1803921569, blue: 0.2862745098, alpha: 1)
        static var sideMenuSeparatorBackgroundColor = #colorLiteral(red: 0.537254902, green: 0.5411764706, blue: 0.5725490196, alpha: 1)
        static var profileImage = UIImage(named: "profileLogoWhite")
        static var sideMenuProfileColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
        static var sideMenuTransparentViewBackgroundColor = #colorLiteral(red: 0.03313663974, green: 0.1527673006, blue: 0.335814476, alpha: 1)
        
        static var PreflightDetailCellHeaderColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
        static var PreflightDetailCellDataColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
        static var PreflightLegViewButtonColor = #colorLiteral(red: 0.2423413098, green: 0.6384810805, blue: 0.8548806906, alpha: 1)
        
        static func Setting_Theme_Update() -> Void{
            if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.THEME) == AppConfig.THEME.DARK.rawValue){
                // BLUE THEME
                AppConfig.AppColor.PostflightDetailBackgroungColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.PostflightDetailCardColor = #colorLiteral(red: 0.03313663974, green: 0.1527673006, blue: 0.335814476, alpha: 1)
                AppConfig.AppColor.PostflightDetailContentLabelColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightWarningContentLabelColor = #colorLiteral(red: 1.0, green: 0.69411765, blue: 0.23529412, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightDetailDataLabelColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)    //Text Color
                AppConfig.AppColor.PostflightDetailCollapseIconColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailTitleTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailBorderColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonBorderColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonTextColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailcellBackgroundColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)
                AppConfig.AppColor.PostflightDetailDeleteButtonBackgroundColor = #colorLiteral(red: 0.09019607843, green: 0.2666666667, blue: 0.4862745098, alpha: 1)
                AppConfig.AppColor.PostflightDetailNotesBackgroundColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)
                AppConfig.AppColor.PostflightDetailSegmentBackgroundColor = #colorLiteral(red: 0.07150798291, green: 0.1474402547, blue: 0.291274488, alpha: 1)
                AppConfig.AppColor.PostflightDetailSelectedSegmentColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailLogbookColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailPassengerStatusColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailCrewCellImageColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailExpenseBackgroundColor = #colorLiteral(red: 0.03313663974, green: 0.1527673006, blue: 0.335814476, alpha: 1)
                AppConfig.AppColor.PostflightDetailCellCardBackgroundColor = #colorLiteral(red: 0.0267236209, green: 0.1306961614, blue: 0.2856808944, alpha: 1)
                AppConfig.AppColor.postflightPersonImageColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightPilotChecklistColor = #colorLiteral(red: 0, green: 0.9098039216, blue: 0.1450980392, alpha: 1)
                AppConfig.AppColor.PostflightSchedulingChecklistColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightMaintenanceChecklistColor = #colorLiteral(red: 0.9647058824, green: 0.3254901961, blue: 0.3254901961, alpha: 1)
                
                AppConfig.AppColor.PostflightDetailSwitchThumbColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOffColor = #colorLiteral(red: 0.3411764801, green: 0.6235294342, blue: 0.1686274558, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOnColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchBackgroundColor = #colorLiteral(red: 0.3411764801, green: 0.6235294342, blue: 0.1686274558, alpha: 1)
                
                AppConfig.AppColor.ThumbColor =  #colorLiteral(red: 0.2495934963, green: 0.5721635222, blue: 0.7182354927, alpha: 1)
                AppConfig.AppColor.CustomSegmentColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.FlightFilterViewBackgroundColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                
                AppConfig.AppColor.PostflightCardColor = #colorLiteral(red: 0.01705119759, green: 0.09755637497, blue: 0.256428808, alpha: 1)
                AppConfig.AppColor.PostflightNavColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.PostflightBackgroundColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.PostflightCellContentTextColor = #colorLiteral(red: 0.9931506515, green: 0.9932894111, blue: 0.9931069016, alpha: 1)
                AppConfig.AppColor.PostflightCellDataTextColor = #colorLiteral(red: 0.9965346456, green: 0.996673882, blue: 0.9964906573, alpha: 1)
                AppConfig.AppColor.PostflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
                AppConfig.AppColor.PostflightCellUTCTextColor = #colorLiteral(red: 0.4806716442, green: 0.564641118, blue: 0.6149906516, alpha: 1)
                //AppConfig.AppColor.PostflightCellCardBackgroundColor = #colorLiteral(red: 0.05696237219, green: 0.1669606065, blue: 0.3701624061, alpha: 1)
                AppConfig.AppColor.PostflightCellUTCTextColor = #colorLiteral(red: 0.5843635798, green: 0.6547639966, blue: 0.6979871392, alpha: 1)
                AppConfig.AppColor.PostflightCellCardBackgroundColor = #colorLiteral(red: 0.0267236209, green: 0.1306961614, blue: 0.2856808944, alpha: 1)
                
                AppConfig.AppColor.preflightCardColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)
                AppConfig.AppColor.preflightNavColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.preflightBackgroundColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.preflightCellContentTextColor = #colorLiteral(red: 0.9931506515, green: 0.9932894111, blue: 0.9931069016, alpha: 1)
                AppConfig.AppColor.preflightCellDataTextColor = #colorLiteral(red: 0.9965346456, green: 0.996673882, blue: 0.9964906573, alpha: 1)
                AppConfig.AppColor.CustomNavTextColor = #colorLiteral(red: 0.9965346456, green: 0.996673882, blue: 0.9964906573, alpha: 1)
                AppConfig.AppColor.preflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
                AppConfig.AppColor.preflightCellUTCTextColor = #colorLiteral(red: 0.5843635798, green: 0.6547639966, blue: 0.6979871392, alpha: 1)
                AppConfig.AppColor.preflightCellMediumHeaderTextColor = #colorLiteral(red: 0.2174475491, green: 0.6580834389, blue: 0.8743098378, alpha: 1)
                AppConfig.AppColor.preflightCellSmallHeaderTextColor = #colorLiteral(red: 0.4305613935, green: 0.7204484344, blue: 0.8278864026, alpha: 1)
                AppConfig.AppColor.preflightCellPhoneNumberTextColor = #colorLiteral(red: 0.5844123363, green: 0.6547577977, blue: 0.7018074989, alpha: 1)
                
                AppConfig.AppColor.PostflightExpenseCellBackgroundColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)
                
                AppConfig.AppColor.popupViewBackgroundColor = #colorLiteral(red: 0.05021624267, green: 0.1232144609, blue: 0.2696128488, alpha: 1)
                AppConfig.AppColor.popupViewTextColor = #colorLiteral(red: 0.9999160171, green: 1, blue: 0.9998719096, alpha: 1)
                AppConfig.AppColor.popupViewStarColor = #colorLiteral(red: 0.8102459311, green: 0.8452447653, blue: 0.8639627099, alpha: 1)
                
                AppConfig.AppColor.filtersBackgroungColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.filtersTextColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                
                AppConfig.AppColor.searchViewBackgroundColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)//#colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.searchViewPlaceHolderColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                
                AppConfig.AppColor.statusbarBackgroundColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                
                AppConfig.AppColor.refreshControlColor = UIColor.white
                
                AppConfig.AppColor.tabBarBackgroundColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.tabBarTextColor = #colorLiteral(red: 0.8658080101, green: 0.8627588153, blue: 0.8625558615, alpha: 1)
                AppConfig.AppColor.selectedTabTextColor = #colorLiteral(red: 0.8658080101, green: 0.8627588153, blue: 0.8625558615, alpha: 1)
                AppConfig.AppColor.tabBarSelectedItemTintColor = #colorLiteral(red: 0.2065613568, green: 0.627014339, blue: 0.8508805633, alpha: 1)
                AppConfig.AppColor.tabBarUnselectedItemTintColor = #colorLiteral(red: 1.0, green: 1.0, blue: 1.0, alpha: 1.0)
                
                AppConfig.AppColor.saveImage = UIImage(named: "saveIconGreen")
                
                AppConfig.AppColor.activityViewStyle = UIActivityIndicatorView(style: .white)
                
                AppConfig.AppColor.flightDetailsMenuBackgroundColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuColor = #colorLiteral(red: 0.01813372225, green: 0.1027547196, blue: 0.2609194219, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuTextColor = #colorLiteral(red: 0.9999960065, green: 1, blue: 1, alpha: 1)
                
                AppConfig.AppColor.settingsNavBarBackgroundColor = #colorLiteral(red: 0.09614961594, green: 0.3285063505, blue: 0.5196019411, alpha: 1)
                AppConfig.AppColor.settingsPageBackgroundColor = #colorLiteral(red: 0.03313663974, green: 0.1527673006, blue: 0.335814476, alpha: 1)
                AppConfig.AppColor.settingsHeaderTextColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.settingsSmallHeaderTextColor = #colorLiteral(red: 0.8931506515, green: 0.8932894111, blue: 0.8931069016, alpha: 1)
                AppConfig.AppColor.settingsTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                AppConfig.AppColor.emailImage = UIImage(named: "emailWhite")
                
                AppConfig.AppColor.sideMenuTitleColor = #colorLiteral(red: 0.6666666667, green: 0.6705882353, blue: 0.7058823529, alpha: 1)
                AppConfig.AppColor.sideMenuOptionTitleColor = #colorLiteral(red: 0.9965346456, green: 0.996673882, blue: 0.9964906573, alpha: 1)
                AppConfig.AppColor.sideMenuSupportCellColor = #colorLiteral(red: 0.1098039216, green: 0.1647058824, blue: 0.2705882353, alpha: 1)
                AppConfig.AppColor.sideMenuBackgroundColor = #colorLiteral(red: 0.09803921569, green: 0.1803921569, blue: 0.2862745098, alpha: 1)
                AppConfig.AppColor.sideMenuSeparatorBackgroundColor = #colorLiteral(red: 0.537254902, green: 0.5411764706, blue: 0.5725490196, alpha: 1)
                AppConfig.AppColor.profileImage = UIImage(named: "profileLogo3")
                AppConfig.AppColor.sideMenuProfileColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                
                AppConfig.AppColor.checklistViewBackgroundColor = #colorLiteral(red: 0.09476391226, green: 0.2012417018, blue: 0.2967114449, alpha: 1)
                AppConfig.AppColor.checklistCellColor = #colorLiteral(red: 0.08076592535, green: 0.1359926462, blue: 0.1905433238, alpha: 1)
                AppConfig.AppColor.PreflightLegViewButtonColor = #colorLiteral(red: 0.2423413098, green: 0.6384810805, blue: 0.8548806906, alpha: 1)
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.THEME) == AppConfig.THEME.LIGHT.rawValue){
                // LIGHT THEME
                AppConfig.AppColor.PostflightDetailBackgroungColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.PostflightDetailContentLabelColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightWarningContentLabelColor = #colorLiteral(red: 1.0, green: 0.69411765, blue: 0.23529412, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightDetailDataLabelColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)    //Text Color
                AppConfig.AppColor.PostflightDetailCollapseIconColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.PostflightDetailTitleTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                AppConfig.AppColor.PostflightDetailBorderColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonBorderColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailcellBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
                AppConfig.AppColor.PostflightDetailDeleteButtonBackgroundColor = #colorLiteral(red: 0.06666667, green: 0.19215686, blue: 0.41960784, alpha: 1)
                AppConfig.AppColor.PostflightDetailNotesBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailSegmentBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
                AppConfig.AppColor.PostflightDetailSelectedSegmentColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailLogbookColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailPassengerStatusColor = #colorLiteral(red: 0.5704585314, green: 0.5704723597, blue: 0.5704649091, alpha: 1)
                AppConfig.AppColor.PostflightDetailCrewCellImageColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.PostflightDetailExpenseBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.PostflightDetailCellCardBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
                AppConfig.AppColor.postflightPersonImageColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.PostflightPilotChecklistColor = #colorLiteral(red: 0, green: 0.9098039216, blue: 0.1450980392, alpha: 1)
                AppConfig.AppColor.PostflightSchedulingChecklistColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightMaintenanceChecklistColor = #colorLiteral(red: 0.9647058824, green: 0.3254901961, blue: 0.3254901961, alpha: 1)
                AppConfig.AppColor.ThumbColor = #colorLiteral(red: 0.331592232, green: 0.6428635716, blue: 0.7931218743, alpha: 1)
                AppConfig.AppColor.CustomSegmentColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.FlightFilterViewBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                
                AppConfig.AppColor.PostflightDetailSwitchThumbColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOffColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOnColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
                
                AppConfig.AppColor.PostflightCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.PostflightNavColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.PostflightBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightCellContentTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
                AppConfig.AppColor.PostflightCellDataTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.PostflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
                AppConfig.AppColor.PostflightCellUTCTextColor = #colorLiteral(red: 0.4806716442, green: 0.564641118, blue: 0.6149906516, alpha: 1)
                AppConfig.AppColor.PostflightCellCardBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
                
                AppConfig.AppColor.preflightCardColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.preflightNavColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.preflightBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.preflightCellContentTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
                AppConfig.AppColor.preflightCellDataTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.CustomNavTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.preflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
                AppConfig.AppColor.preflightCellUTCTextColor = #colorLiteral(red: 0.4806716442, green: 0.564641118, blue: 0.6149906516, alpha: 1)
                AppConfig.AppColor.preflightCellMediumHeaderTextColor = #colorLiteral(red: 0.2065613568, green: 0.627014339, blue: 0.8508805633, alpha: 1)
                AppConfig.AppColor.preflightCellSmallHeaderTextColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.preflightCellPhoneNumberTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                
                AppConfig.AppColor.PostflightExpenseCellBackgroundColor = #colorLiteral(red: 0.7290424705, green: 0.7673264146, blue: 0.8384597301, alpha: 1)
                
                AppConfig.AppColor.popupViewBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.popupViewTextColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.popupViewStarColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                
                AppConfig.AppColor.filtersBackgroungColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.filtersTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
                
                AppConfig.AppColor.searchViewBackgroundColor = #colorLiteral(red: 0.8810480237, green: 0.887044251, blue: 0.9233464003, alpha: 1)
                AppConfig.AppColor.searchViewPlaceHolderColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                
                AppConfig.AppColor.statusbarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                
                AppConfig.AppColor.refreshControlColor = UIColor.gray
                
                AppConfig.AppColor.tabBarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.tabBarTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.selectedTabTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.tabBarSelectedItemTintColor = #colorLiteral(red: 0.2065613568, green: 0.627014339, blue: 0.8508805633, alpha: 1)
                AppConfig.AppColor.tabBarUnselectedItemTintColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                
                AppConfig.AppColor.saveImage = UIImage(named: "saveIcon")
                
                AppConfig.AppColor.activityViewStyle = UIActivityIndicatorView(style: .gray)
                
                AppConfig.AppColor.flightDetailsMenuBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuColor = #colorLiteral(red: 0.2885026634, green: 0.6937652826, blue: 0.8812468648, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuTextColor = #colorLiteral(red: 0.9999960065, green: 1, blue: 1, alpha: 1)
                
                AppConfig.AppColor.settingsNavBarBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.settingsPageBackgroundColor = #colorLiteral(red: 0.9620770812, green: 0.9725568891, blue: 1, alpha: 1)
                AppConfig.AppColor.settingsHeaderTextColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.settingsSmallHeaderTextColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
                AppConfig.AppColor.settingsTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                AppConfig.AppColor.emailImage = UIImage(named: "emailBlack")
                
                AppConfig.AppColor.sideMenuTitleColor = #colorLiteral(red: 0.6666666667, green: 0.6705882353, blue: 0.7058823529, alpha: 1)
                AppConfig.AppColor.sideMenuOptionTitleColor = #colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.sideMenuSupportCellColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.sideMenuBackgroundColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                AppConfig.AppColor.sideMenuSeparatorBackgroundColor = #colorLiteral(red: 0.537254902, green: 0.5411764706, blue: 0.5725490196, alpha: 1)
                AppConfig.AppColor.profileImage = UIImage(named: "profileLogoWhite")
                AppConfig.AppColor.sideMenuProfileColor = #colorLiteral(red: 0.1928167343, green: 0.1960446537, blue: 0.2249017656, alpha: 1)
                
                AppConfig.AppColor.checklistViewBackgroundColor = #colorLiteral(red: 0.09476391226, green: 0.2012417018, blue: 0.2967114449, alpha: 1)
                AppConfig.AppColor.checklistCellColor = #colorLiteral(red: 0.08076592535, green: 0.1359926462, blue: 0.1905433238, alpha: 1)
                AppConfig.AppColor.PreflightLegViewButtonColor = #colorLiteral(red: 0.2423413098, green: 0.6384810805, blue: 0.8548806906, alpha: 1)
            }else if (UserDefaults.standard.integer(forKey: AppConfig.UserdefaultKeys.THEME) == AppConfig.THEME.DARKNIGHT.rawValue){
                // DARKNIGHT THEME
                AppConfig.AppColor.PostflightDetailBackgroungColor = #colorLiteral(red: 0.0355640538, green: 0.07665470988, blue: 0.1514765918, alpha: 1)
                AppConfig.AppColor.PostflightDetailCardColor = #colorLiteral(red: 0.1076258197, green: 0.16268906, blue: 0.2685910165, alpha: 1)
                AppConfig.AppColor.PostflightDetailContentLabelColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightWarningContentLabelColor = #colorLiteral(red: 1.0, green: 0.69411765, blue: 0.23529412, alpha: 1) //Text Color
                AppConfig.AppColor.PostflightDetailDataLabelColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)    //Text Color
                AppConfig.AppColor.PostflightDetailCollapseIconColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightDetailTitleTextColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailBorderColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonBorderColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightDetailAddButtonTextColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightDetailcellBackgroundColor = #colorLiteral(red: 0.1804355979, green: 0.23258394, blue: 0.3291217089, alpha: 1)
                AppConfig.AppColor.PostflightDetailDeleteButtonBackgroundColor = #colorLiteral(red: 0.7296414375, green: 0.1240646914, blue: 0.1935257912, alpha: 1)
                AppConfig.AppColor.PostflightDetailNotesBackgroundColor = #colorLiteral(red: 0.1804355979, green: 0.23258394, blue: 0.3291217089, alpha: 1)
                AppConfig.AppColor.PostflightDetailSegmentBackgroundColor = #colorLiteral(red: 0.1804355979, green: 0.23258394, blue: 0.3291217089, alpha: 1)
                AppConfig.AppColor.PostflightDetailSelectedSegmentColor = #colorLiteral(red: 0.1624448001, green: 0.4947847128, blue: 0.7801012993, alpha: 1)
                AppConfig.AppColor.PostflightDetailLogbookColor = #colorLiteral(red: 0.1624448001, green: 0.4947847128, blue: 0.7801012993, alpha: 1)
                AppConfig.AppColor.PostflightDetailPassengerStatusColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightDetailCrewCellImageColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightDetailExpenseBackgroundColor = #colorLiteral(red: 0.1076258197, green: 0.16268906, blue: 0.2685910165, alpha: 1)
                AppConfig.AppColor.PostflightDetailCellCardBackgroundColor = #colorLiteral(red: 0.07139300555, green: 0.07140972465, blue: 0.07138773054, alpha: 1)
                AppConfig.AppColor.postflightPersonImageColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightPilotChecklistColor = #colorLiteral(red: 0, green: 0.9098039216, blue: 0.1450980392, alpha: 1)
                AppConfig.AppColor.PostflightSchedulingChecklistColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.PostflightMaintenanceChecklistColor = #colorLiteral(red: 0.9647058824, green: 0.3254901961, blue: 0.3254901961, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchThumbColor = #colorLiteral(red: 0.50196078, green: 0.81960784, blue: 0.91764706, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOffColor = #colorLiteral(red: 0.1096091047, green: 0.153124243, blue: 0.2128463984, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchOnColor = #colorLiteral(red: 0.69411765, green: 0.89019608, blue: 0.94901961, alpha: 1)
                AppConfig.AppColor.PostflightDetailSwitchBackgroundColor = #colorLiteral(red: 0.1096091047, green: 0.153124243, blue: 0.2128463984, alpha: 1)
                
                AppConfig.AppColor.ThumbColor =  #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.CustomSegmentColor = #colorLiteral(red: 0.09775102884, green: 0.1817699075, blue: 0.2818114758, alpha: 1)
                AppConfig.AppColor.FlightFilterViewBackgroundColor = #colorLiteral(red: 0.1370117366, green: 0.1628406942, blue: 0.2027286887, alpha: 1)
                
                AppConfig.AppColor.PostflightCardColor = #colorLiteral(red: 0.1076258197, green: 0.16268906, blue: 0.2685910165, alpha: 1)
                AppConfig.AppColor.PostflightNavColor = #colorLiteral(red: 0.1072005555, green: 0.1864625812, blue: 0.2951670289, alpha: 1)
                AppConfig.AppColor.PostflightBackgroundColor = #colorLiteral(red: 0.03841544315, green: 0.09259500355, blue: 0.1710685194, alpha: 1)
                AppConfig.AppColor.PostflightCellContentTextColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightCellDataTextColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.PostflightCellTripIDnoColor = #colorLiteral(red: 0.8776615262, green: 0.8870767951, blue: 0.9016378522, alpha: 1)
                AppConfig.AppColor.PostflightCellUTCTextColor = #colorLiteral(red: 0.4046860933, green: 0.4439043701, blue: 0.5146163106, alpha: 1)
                //AppConfig.AppColor.PostflightCellCardBackgroundColor = #colorLiteral(red: 0.05696237219, green: 0.1669606065, blue: 0.3701624061, alpha: 1)
                AppConfig.AppColor.PostflightCellUTCTextColor = #colorLiteral(red: 0.4046860933, green: 0.4439043701, blue: 0.5146163106, alpha: 1)
                AppConfig.AppColor.PostflightCellCardBackgroundColor = #colorLiteral(red: 0.1076258197, green: 0.16268906, blue: 0.2685910165, alpha: 1)
                
                AppConfig.AppColor.preflightCardColor = #colorLiteral(red: 0.1076258197, green: 0.16268906, blue: 0.2685910165, alpha: 1)
                AppConfig.AppColor.preflightNavColor = #colorLiteral(red: 0.1072005555, green: 0.1864625812, blue: 0.2951670289, alpha: 1)
                AppConfig.AppColor.preflightBackgroundColor = #colorLiteral(red: 0.03974581882, green: 0.09779954702, blue: 0.1856591403, alpha: 1)
                AppConfig.AppColor.preflightCellContentTextColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.preflightCellDataTextColor = #colorLiteral(red: 0.8811348081, green: 0.8905427456, blue: 0.9050913453, alpha: 1)
                AppConfig.AppColor.CustomNavTextColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.preflightCellTripIDnoColor = #colorLiteral(red: 0.797070682, green: 0.8039149642, blue: 0.83691293, alpha: 1)
                AppConfig.AppColor.preflightCellUTCTextColor = #colorLiteral(red: 0.4046860933, green: 0.4439043701, blue: 0.5146163106, alpha: 1)
                AppConfig.AppColor.preflightCellMediumHeaderTextColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.preflightCellSmallHeaderTextColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.preflightCellPhoneNumberTextColor = #colorLiteral(red: 0.5844123363, green: 0.6547577977, blue: 0.7018074989, alpha: 1)
                
                AppConfig.AppColor.PostflightExpenseCellBackgroundColor = #colorLiteral(red: 0.1804355979, green: 0.23258394, blue: 0.3291217089, alpha: 1)
                
                AppConfig.AppColor.popupViewBackgroundColor = #colorLiteral(red: 0.1370117366, green: 0.1628406942, blue: 0.2027286887, alpha: 1)
                AppConfig.AppColor.popupViewTextColor = #colorLiteral(red: 0.9999160171, green: 1, blue: 0.9998719096, alpha: 1)
                AppConfig.AppColor.popupViewStarColor = #colorLiteral(red: 0.8102459311, green: 0.8452447653, blue: 0.8639627099, alpha: 1)
                
                AppConfig.AppColor.filtersBackgroungColor = #colorLiteral(red: 0.1370117366, green: 0.1628406942, blue: 0.2027286887, alpha: 1)
                AppConfig.AppColor.filtersTextColor = #colorLiteral(red: 0.9265142679, green: 0.9333273172, blue: 0.9704359174, alpha: 1)
                
                AppConfig.AppColor.searchViewBackgroundColor = #colorLiteral(red: 0.1096091047, green: 0.153124243, blue: 0.2128463984, alpha: 1)//#colorLiteral(red: 0.3068887293, green: 0.313698113, blue: 0.3426089883, alpha: 1)
                AppConfig.AppColor.searchViewPlaceHolderColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                
                AppConfig.AppColor.statusbarBackgroundColor = #colorLiteral(red: 0.1072005555, green: 0.1864625812, blue: 0.2951670289, alpha: 1)
                
                AppConfig.AppColor.refreshControlColor = UIColor.white
                
                AppConfig.AppColor.tabBarBackgroundColor = #colorLiteral(red: 0, green: 0, blue: 0, alpha: 1)
                AppConfig.AppColor.tabBarTextColor = #colorLiteral(red: 0.8658080101, green: 0.8627588153, blue: 0.8625558615, alpha: 1)
                AppConfig.AppColor.selectedTabTextColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.tabBarSelectedItemTintColor = #colorLiteral(red: 0.5996178985, green: 0.8903332353, blue: 1, alpha: 1)
                AppConfig.AppColor.tabBarUnselectedItemTintColor = #colorLiteral(red: 1.0, green: 1.0, blue: 1.0, alpha: 1.0)
                
                AppConfig.AppColor.saveImage = UIImage(named: "saveIconBlue1")
                
                AppConfig.AppColor.activityViewStyle = UIActivityIndicatorView(style: .white)
                
                AppConfig.AppColor.flightDetailsMenuBackgroundColor = #colorLiteral(red: 0.0355640538, green: 0.07665470988, blue: 0.1514765918, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuColor = #colorLiteral(red: 0.09258895367, green: 0.1770458221, blue: 0.2728743255, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuColor = #colorLiteral(red: 0.0, green: 0.68235294, blue: 0.9372549, alpha: 1)
                AppConfig.AppColor.flightDetailsMenuTextColor = #colorLiteral(red: 0.9999960065, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.flightDetailsSelectedMenuTextColor = #colorLiteral(red: 0.0, green: 0.0, blue: 0.0, alpha: 1)
                
                AppConfig.AppColor.settingsNavBarBackgroundColor = #colorLiteral(red: 0.1072005555, green: 0.1864625812, blue: 0.2951670289, alpha: 1)
                AppConfig.AppColor.settingsPageBackgroundColor = #colorLiteral(red: 0.03588426486, green: 0.0925674215, blue: 0.1809056997, alpha: 1)
                AppConfig.AppColor.settingsHeaderTextColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 0.9)
                AppConfig.AppColor.settingsSmallHeaderTextColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 0.7)
                AppConfig.AppColor.settingsTextColor = #colorLiteral(red: 0.619554162, green: 0.6196434498, blue: 0.6195260286, alpha: 1)
                AppConfig.AppColor.emailImage = UIImage(named: "emailWhite")
                
                AppConfig.AppColor.sideMenuTitleColor = #colorLiteral(red: 0.4464936256, green: 0.4992168546, blue: 0.5605930686, alpha: 1)
                AppConfig.AppColor.sideMenuOptionTitleColor = #colorLiteral(red: 0.639844954, green: 0.6695110202, blue: 0.7158995867, alpha: 1)
                AppConfig.AppColor.sideMenuSupportCellColor = #colorLiteral(red: 0.10980392, green: 0.16470588, blue: 0.27058824, alpha: 1)
                AppConfig.AppColor.sideMenuBackgroundColor = #colorLiteral(red: 0.09803922, green: 0.18039216, blue: 0.28235294, alpha: 1)
                AppConfig.AppColor.sideMenuSeparatorBackgroundColor = #colorLiteral(red: 0.4464936256, green: 0.4992168546, blue: 0.5605930686, alpha: 1)
                AppConfig.AppColor.profileImage = UIImage(named: "profileLogo3")
                AppConfig.AppColor.sideMenuProfileColor = #colorLiteral(red: 0.6435534358, green: 0.6731863618, blue: 0.7195243239, alpha: 1)
                
                AppConfig.AppColor.checklistViewBackgroundColor = #colorLiteral(red: 0.09476391226, green: 0.2012417018, blue: 0.2967114449, alpha: 1)
                AppConfig.AppColor.checklistCellColor = #colorLiteral(red: 0.08076592535, green: 0.1359926462, blue: 0.1905433238, alpha: 1)
                
                AppConfig.AppColor.PreflightDetailCellHeaderColor = #colorLiteral(red: 0.6, green: 0.8901960784, blue: 1, alpha: 1)
                AppConfig.AppColor.PreflightDetailCellDataColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                AppConfig.AppColor.PreflightLegViewButtonColor = #colorLiteral(red: 0.2423413098, green: 0.6384810805, blue: 0.8548806906, alpha: 1)
            }
        }
    }
    static var PostFlightDetailContent = [["OOOI",["basic","departure/arivalICAO","departureDate","arrivalDate","out/off","on/in"],[75.0,70.0,61.0,61.0,70.0,70.0]],
                                          ["FUEL",["basic","burn","out/off","on/in","fuel burn"/*,"uplift","planned/actual"*/],[55.0,100.0,70.0,70.0,61.0/*,100.0,70.0*/]], ["CREW",["basic"/*"pilotSecName","basicPilot","otherSecName","basicOther"*/,"addPilot","crewSum","role","time","track/hold","takeoffs","landings","approachTitle","approach","addApproach","empty"],[44.0/*20.0,50.0,20.0,20.0*/,60.0,140.0,67.0,70.0,60.0,110.0,110.0,44.0,80.0,60.0,13.0]],
                                          ["PASSENGERS",["basic"/*,"addPassengers"*/,"passengersName"/*, "passengersStatus"*/,"empty"],[44.0/*,60.0*/,50.0/*, 78.5*/,13.0]],
                                          ["ADDITIONAL DETAILS",["basic","businessCatagory/charge","type/duration","goAround/rejectedTakeOff"],[70.0,70.0,70.0,70.0]],
                                          ["NOTES",[""],[73.0]],// 19th Dec 2019
                                          ["EXPENSES",["basic","addExpenses","expenseReceipt","empty"],[44.0,60.0,82.0,13.0]],// 20th Feb 2020
                                          ["DOCUMENTS",["basic","addDocuments","documentReceipt","empty"],[44.0,60.0,82.0,13.0]],
                                          ["DE/ANTI-ICE",["basic","startDate/startTime","endDate/endTime","ratio/type"],[50.0,70.0,70.0,70.0]],
                                          ["APU & CUSTOM COMPONENT(S)",["basic","basicContent","basicMessage","type","element"],[44.0,45.0,50.0,30.0,70.0]],
                                          ["OILS & FLUIDS",["basic","basicContent","basicMessage","unit","amount"],[44.0,35.0,50.0,100.0,70.0]],
                                          ["SQUAWKS & DISCREPANCIES",["basic","squawksBrief","addSquawk"/*,"squawkSum"*/,"date","ataCode","discrepencyType","reportedBy","category","descrption","empty"],[44.0,100.0,60.0/*,70.0*/,70.0,70.0,70.0,70.0,70.0,70.0,13.0]],
                                          ["DUTY TIME",["basic","addDutyTime","dutySum","crewname","startDate/startTime","endDate/endTime","ron/duty","empty"],[44.0,60.0,70,70.0,70.0,70.0,50.0,13.0]]]
    static var iSSettingVisivilityUpdate = false
    static var isTabChanged = false
    static var isFreshLunch = false
    static var isFreshLogin = false
    static var isChecklistUdated = false
    static var deviceToken = ""
}
