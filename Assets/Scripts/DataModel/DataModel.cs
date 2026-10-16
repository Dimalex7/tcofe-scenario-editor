using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.VisualScripting;

//------- ENUMS ------- ------- ------- ------- ------- -------
[Serializable]
public enum RepeatMode {
    REPEAT= 1, //Repeat with COST, TRIGGER performance and HISTORY record inserted in 'performedActionRecords'
    NO_REPEAT = 2, //REPEAT is not allowed
    REPEAT_WITH_COST_TRIGGER = 3, //No HISTORY in 'performedActionRecords'
    REPEAT_WITH_COST = 4, //No TRIGGER & HISTORY in 'performedActionRecords'
    REPEAT_WITH_TRIGGER = 5, //No COST, No HISTORY in 'performedActionRecords'
    REPEAT_WITH_TRIGGER_HISTORY = 6, //No COST
    REPEAT_WITH_HISTORY = 7, //No COST, TRIGGRER, HISTORY in 'performedActionRecords'
    REPEAT_WITHOUT_COST_TRIGGER_AND_HISTORY = 8, //No COST
    UNDEFINED = -1
}//RepeatMode

[Serializable]
public enum AdvanceTimeMode {
    ADVANCETIME = 1,
    NOTADVANCETIME = 2,
    UNDEFINED = -1
}

[Serializable]
public enum AssetType {
    //-------- BASICS & GENERALYS (0 .. 24) ------- ------- -------
    INFO = 1,
    NETWORK = 2,
    WORKSTATIONS_POOL = 3,
    WORKSTATION = 4,
    ROUTER = 5,
    FIREWALL = 6,
    POLICY_CENTER = 7,
    REPORT = 8,
    LOGS = 9,
    TRAINING_RECORDS = 10,
    WATCHBOARD_TASK = 11,
    HW = 22,
    SW = 23,
    ALL = 24, // => 'ALL_ASSETS' used when it has to be declared something
    //-------- SERVER (25 .. 49) ------- ------- -------
    SERVER = 25,
    APPLICATION_SERVER = 26,
    WEB_SERVER = 27,
    DATABASE_SERVER = 28,
    FILE_SERVER = 29,
    EMAIL_SERVER = 30,
    VPN_SERVER = 31,
    //-------- DEVICES (50 .. 74) ------- ------- -------
    DEVICE = 50,
    NETWORK_DEVICE = 51,
    MOBILE_DEVICE = 52,
    IOT_DEVICE = 53,
    PRINTER = 54,
    LAN_DEVICE = 55,
    WIFI_DEVICE = 56,
    VIRTUAL_MACHINE = 57,
    SWITCH = 58,
    ICS = 59,
    SCADA_SYSTEM = 60,
    NAS = 61,
    STORAGE_DEVICE = 62,
    NETWORK_PRINTER = 63,
    ACCESS_POINT = 64,
    //-------- SERVICES (75 .. 99) ------- ------- -------
    SERVICE = 75,
    APPLICATION = 76,
    CLOUD_SERVICE = 77,
    //-------- SECURITY SOLUTIONS (100 .. 124) ------- ------- -------
    CS_SOLUTION = 100,
    ANTIVIRUS = 101,
    IDS = 102,
    IPS = 103,
    SIEM = 104,
    //-------- IDENTITY & ACCESS (125 .. 149) ------- ------- -------
    ACCESS_CONFS_AND_POLICIES = 125,
    ACTIVE_DIRECTORY = 126,
    MFA_POLICY = 127,
    DOMAIN_CONTROLLER = 128,
    IDENTITY_SERVICE = 129,
    PASSWORD_RESET = 130,
    FIREWALL_RULE = 131,
    /*DATABASE = 6*/
    //-------- CHARACTERS (150 .. 174) ------- ------- -------
    CHARACTERS = 150,
    LEARNER = 151, //Used when it has to be declared something
    NPC = 152,
    EMPLOYEES_ALL = 153,
    ADMINS = 154,
    SECRETARY = 155,
    JOURNALIST = 156,
    //Aligned with ReportAudience
    CEO = 161,
    CFO = 162,
    CEO_AND_CFO = 163,
    BOARD = 164,
    LEGAL_DEPARTMENT = 165,
    DPO = 166,
    EMPLOYEES = 167,
    IT_OPERATIONS = 168,
    IR_TEAM = 169,
    MEDIA = 170,
    SOC = 171,
    //-------- ACCOUNTS (175 .. 199) ------- ------- -------
    ACCOUNT = 175,
    LEARNER_ACCOUNT = 176,
    ALL_ACCOUNTS = 177,
    ADMIN_ACCOUNT = 178,
    ADMIN_ACCOUNTS = 179,
    SECRETARY_ACCOUNT = 180,
    CEO_ACCOUNT = 181,
    CFO_ACCOUNT = 182,
    GUEST_ACCOUNT = 183,
    //-------- Vulnerabilities (200 .. 249) ------- ------- -------
    VULNERABILITY = 200,
    //-------- Threats (250 .. 274) ------- ------- -------
    THREAT = 250,
    //-------- System (275 .. ) ------- ------- -------
    SYSTEM = 275,
    //-------- GENERAL ------- ------- -------    
    UNDEFINED = -1
}//AssetType

[Serializable]
public enum AuditTags{
    INSPECTED = 1, //an Inspectable asset that has been inspected
    TAGGED = 2, //a Taggable asset that has been tagged
    CRITICAL = 3, //with high BI impact), etc.
    UNDEFINED = -1
}//AuditTags

[Serializable]
public enum Capability {
    INSPECTABLE = 1, //an asset that can be inspected
    PATCHABLE = 2,
    ISOLATABLE = 3,
    RESTORABLE = 4,
    TAGGABLE = 5, //an asset that can be tagged
    FIRMWARE_UPDATABLE = 6,
    KEEPS_LOGS = 7,
    VULNERABLE = 8,
    UNDEFINED = -1
}//Capabilities

[Serializable]
public enum CofeletOntologyType {
    TRIPLE = 1,
    QUADRUPLE = 2,
    QUINTUPLE = 3,
    UNDEFINED = -1
}

[Serializable]
public enum EvidenceType {
    INSPECTED = 1,
    VULNERABILITY = 2, //OR WEAKNESS
    TAGGED = 3,
    HARDENED_ACTION = 4,
    MATURE_ACTION_LEVEL1 = 5,
    MATURE_ACTION_LEVEL2 = 6,
    SCOPE = 7,
    WEAK_POLICY = 8,
    UNDEFINED = -1
}

[Serializable]
public enum GoalType {
    ACTION_SEQUENCE_SPECIFIC = 1,
    ACTION_SEQUENCE_SPECIFIC_ANY_ORDER = 2,
    ACTIONS_SPECIFIC_TYPE_COUNT = 3,
    EVIDENCE_TYPE_COUNT = 4,
    REPORT_COUNT_OF_SPECIFIC_ATTRIBUTES = 5,
    CS_THRESHOLD = 6,
    BI_THRESHOLD = 7,
    BUDGET_THRESHOLD = 8,
    TRUST_THRESHOLD = 9,
    SUSTAINABLE_BI_ABOVE_THRESHOLD = 10,
    SUSTAINABLE_CS_ABOVE_THRESHOLD = 11,
    UNDEFINED = -1
}//GoalType

[Serializable]
public enum OperationalState {
    ACTIVE = 1, //means also NETWORKED/CONNECTED .vs #ISOLATED
    DISABLED = 2, //by an incident
    PAUSED = 3, //by learner
    RESUMING = 4, //period after a pause, before ACTIVE
    RESTORING = 5, //previous state or version from a backup or a snapshot
    RESTARTING = 6, //after the action of Restarting
    TURNED_OFF = 7,
    ISOLATED = 8,
    NETWORKED = 9,
    UNDEFINED = -1
}//OperationalState

[Serializable]
public enum Predicate {
    NOT = 1,
    AND = 2,
    OR = 3,
    XOR = 4,
    UNDEFINED = -1
}//Predicate

public enum Property {
    INSPECT = 1,
    FIND = 2,
    SET = 3,
    ADD = 4,
    REQUEST = 5,
    CORRELATE = 6,
    TURNOFF = 7,
    CREATE = 8,
    ISOLATE = 9,
    CONNECT = 10,
    TURNON = 11,
    CHECK = 12,
    TRAIN = 13,
    VULNERABILITY_SCAN = 14,
    ACKNOWLEDGE = 15,
    UNDEFINED = -1
}//Property

[Serializable]
public enum ReportAudience {
    CEO = 1,
    CFO = 2,
    CEO_AND_CFO = 3,
    BOARD = 4,
    LEGAL_DEPARTMENT = 5,
    DPO = 6,
    EMPLOYEES = 7,
    IT_OPERATIONS = 8,
    IR_TEAM = 9,
    MEDIA = 10,
    SOC = 11,
    UNDEFINED = -1
}//ReportAudience

[Serializable]
public enum ReportDetailLevel {
    ABSTRACT = 1,
    COMPREHENSIVE = 2,
    DETAILED = 3, //+1 AP Cost
    UNDEFINED = -1
}//ReportDetailLevel

[Serializable]
public enum ReportScope {
    INTERNAL = 1,
    EXTERNAL = 2, //+1 AP Cost
    PUBLIC = 3, //+1 AP Cost
    UNDEFINED = -1
}//ReportScope

[Serializable]
public enum ReportTechnicality {
    TECHNICAL = 1, //+1 AP Cost
    NON_TECHNICAL = 2,
    UNDEFINED = -1
}//ReportTechnicality

[Serializable]
public enum ReportTone {
    NEUTRAL = 1,
    FRIENDLY = 2,
    FORMAL = 3,
    UNDEFINED = -1
}//ReportTone

[Serializable]
public enum ReportType {
    NOTIFICATION = 1,
    SUMMARY = 2,
    FULL_REPORT = 3,
    UNDEFINED = -1
}//ReportType

[Serializable]
public enum RoundPhase {
    NOW = 1,
    START_OF_SESSION = 2,
    MID_OF_CURRENT_STEPROUND = 3,
    END_OF_CURRENT_STEPROUND = 4,
    START_OF_STEPROUND_1 = 5,
    MID_OF_STEPROUND_1 = 6,
    END_OF_STEPROUND_1 = 7,
    START_OF_STEPROUND_2 = 8,
    MID_OF_STEPROUND_2 = 9,
    END_OF_STEPROUND_2 = 10,
    START_OF_STEPROUND_3 = 11,
    MID_OF_STEPROUND_3 = 12,
    END_OF_STEPROUND_3 = 13,
    START_OF_STEPROUND_4 = 14,
    MID_OF_STEPROUND_4 = 15,
    END_OF_STEPROUND_4 = 16,
    START_OF_STEPROUND_5 = 17,
    MID_OF_STEPROUND_5 = 18,
    END_OF_STEPROUND_5 = 19,
    START_OF_STEPROUND_6 = 20,
    MID_OF_STEPROUND_6 = 21,
    END_OF_STEPROUND_6 = 22,
    START_OF_STEPROUND_7 = 23,
    MID_OF_STEPROUND_7 = 24,
    END_OF_STEPROUND_7 = 25,
    START_OF_STEPROUND_8 = 26,
    MID_OF_STEPROUND_8 = 27,
    END_OF_STEPROUND_8 = 28,
    START_OF_STEPROUND_9 = 29,
    MID_OF_STEPROUND_9 = 30,
    END_OF_STEPROUND_9 = 31,
    START_OF_STEPROUND_10 = 32,
    MID_OF_STEPROUND_10 = 33,
    END_OF_STEPROUND_10 = 34,
    START_OF_STEPROUND_11 = 35,
    MID_OF_STEPROUND_11 = 36,
    END_OF_STEPROUND_11 = 37,
    START_OF_STEPROUND_12 = 38,
    MID_OF_STEPROUND_12 = 39,
    END_OF_STEPROUND_12 = 40,
    START_OF_STEPROUND_13 = 41,
    MID_OF_STEPROUND_13 = 42,
    END_OF_STEPROUND_13 = 43,
    START_OF_STEPROUND_14 = 44,
    MID_OF_STEPROUND_14 = 45,
    END_OF_STEPROUND_14 = 46,
    START_OF_STEPROUND_15 = 47,
    MID_OF_STEPROUND_15 = 48,
    END_OF_STEPROUND_15 = 49,
    SCHEDULED_TASK_COMPLETED = 50,
    END_OF_SESSION = 100,
    UNDEFINED = -1
}//RoundPhase

[Serializable]
public enum SecurityState {
    SECURED = 1,//means that CS is set to high levels and there is low possibility to be affected by an incident
    MODERATELY_VULNERABLE = 2, //a moderate possibility to be affected by an incident
    HIGHLY_VULNERABLE = 3, //a high possibility to be affected by an incident
    COMPROMISED = 4, //asset has been affected by an incident but it is functional
    ISOLATED = 5, //an asset that has been Isolated from networks and other assets, but it is still functional
    NON_FUNCTIONAL = 6, //an asset that has been affected by an incident and it is not functional
    DESTROYED = 7, //an asset that has been affected by an incident and it is not functional and it is not recoverable, it is destroyed
    UNDEFINED = -1
}//SecurityState

[Serializable]
public enum Severity{
    INFO = 1,
    WARNING = 2,
    CRITICAL = 3,
    NEUTRAL = 4,
    UNDEFINED = -1
}//WatchBoardItemType

[Serializable]
public enum TriggerType {
    //--- PRESENT INFO (1 .. 9) ---
    PRESENT_TRIGGER_INFO = 1,
    PRESENT_ASSET_INFO = 2,
    PRESENT_HINT = 3,
    PRESENT_IMAGE = 4,
    //--- METRICS (10 .. 24) ---
    CHANGE_CS = 10,
    CHANGE_BI = 11,
    CHANGE_BUDGET = 12,
    CHANGE_APS = 13,
    //--- STATE / VISUAL (25 .. 74) ---
    SUBJECT_OPERATIONAL_STATE_CHANGE = 25,
    SUBJECT_SECURITY_STATE_CHANGE = 26,
    SUBJECT_STATE_CHANGE = 27,
    SUBJECT_IMAGE_CHANGE = 28,
    SUBJECT_INFO_CHANGE = 29, //Short information, e.g., presented in a tooltip
    SUBJECT_DESCRIPTION_CHANGE = 30, //More extended information e.g., presented in a panel or a card
    SUBJECT_EFFECT1 = 31,
    OBJECT_OPERATIONAL_STATE_CHANGE = 32,
    OBJECT_SECURITY_STATE_CHANGE = 33,
    OBJECT_STATE_CHANGE = 34,
    OBJECT_IMAGE_CHANGE = 35,
    OBJECT_INFO_CHANGE = 36,
    OBJECT_DESCRIPTION_CHANGE = 37,
    OBJECT_EFFECT1 = 38,
    SOURCE_OPERATIONAL_STATE_CHANGE = 39,
    SOURCE_SECURITY_STATE_CHANGE = 40,
    SOURCE_STATE_CHANGE = 41,
    SOURCE_IMAGE_CHANGE = 42,
    SOURCE_INFO_CHANGE = 43,
    SOURCE_DESCRIPTION_CHANGE = 44,
    SOURCE_EFFECT1 = 45,
    TARGET_OPERATIONAL_STATE_CHANGE = 46,
    TARGET_SECURITY_STATE_CHANGE = 47,
    TARGET_STATE_CHANGE = 48,
    TARGET_IMAGE_CHANGE = 49,
    TARGET_INFO_CHANGE = 50,
    TARGET_DESCRIPTION_CHANGE = 51,
    TARGET_EFFECT1 = 52,
    //--- WATCHBOARD TRIGGERS (75 .. 89) ---
    ADD_FLASHNEWS = 75,
    ADD_THREAT = 76,
    ADD_SCHEDULED_TASK = 77,
    UNLOCK_SCHEDULEDTASK_CHECKBOX = 78,
    CHECK_SCHEDULEDTASK_CHECKBOX = 79,
    COMPLETE_GOAL = 80,
    //--- EVIDENCE REPOSITORY / STAKEHOLDER (90 .. 99) ---
    ADD_EVIDENCE = 90,
    CHANGE_STAKEHOLDER_TRUST = 91,
    PRESENT_VULNERABILITIES_EVIDENCE = 92,
    //--- ACTIONS (100 .. 114) ---
    PERFORM_ACTION_BY_ID = 100,
    ACTION_END_BY_ID = 101,
    ADD_ACTION_BY_ID = 102,
    HIDE_ACTION_BY_ID = 103,
    //--- ADD ASSETS (115 .. 124) ---
    ADD_ASSET_BY_ID = 115,
    ADD_REPORT_ASSET = 116,
    //--- REMOVE ASSETS (125 .. 149) ---
    REMOVE_ASSET_BY_ID = 125,
    //--- ROUND MANAGEMENT (150 .. 174) ---
    START_NEXT_ROUND = 150,
    END_CURRENT_ROUND = 151,
    END_THE_SESSION = 152,
    UNDEFINED = -1
}

[Serializable]
public enum WatchBoardItemType{
    TASK = 1,
    NEWS = 2,
    THREAT = 3,
    UNDEFINED = -1
}//WatchBoardItemType

[Serializable]
public enum WatchBoardItemStatus{
    ACTIVE = 1,
    INACTIVE = 2,
    PERFORMED = 3,
    EXPIRED = 4,
    HIDDEN = 5,
    UNDEFINED = -1
}//WatchBoardItemType

//------- ------- ------- ------- ------- ------- -------

//------- CLASSES ------- ------- ------- ------- -------
[Serializable]
public class ActionTexts {
    public string id;
    public string title;
}

[Serializable]
public class Action: ActionTexts {
    public string baseActionID;
    public CofeletOntologyType ontologyType;
    public string subjectID;
    public AssetType subjectType;
    public Property property;
    public string objectID;
    public List<AssetType> objectTypes = new List<AssetType>();
    public string sourceID;
    public List<AssetType> sourceTypes = new List<AssetType>();
    public string targetID;
    public List<AssetType> targetTypes = new List<AssetType>();
    public List<Capability> requiredCapabilities = new List<Capability>();
    public List<OperationalState> requiredOperationalStates = new List<OperationalState>();
    public List<SecurityState> requiredSecurityStates = new List<SecurityState>();
    public int apCost;
    public int budgetCost;
    public int numOfRoundsForCostDistribution; //0 for immediate charge, 1 for next Round charge, 2 for next 2 Rounds charge. . .
    public int startInSecs; //If not now, when action starts in seconds. Mutual Exclusive with roundPhaseStart.
    public RoundPhase roundPhaseStart; //If not now, when action starts in round phase terms. Mutual Exclusive with startInSecs.
    public int durationInSecs; //Duration in seconds
    public AdvanceTimeMode advancetime;
    public int durationInRounds; //Duration in rounds
    public RoundPhase roundPhaseEnd; //When action ends in round phase (mutual exclusive with durationInSecs, can be used with durationInRounds)
    public RepeatMode actionRepeatMode;
    public List<Trigger> triggers = new List<Trigger>();
    public bool visible;
    public int order;
}//Action

[Serializable]
public class AssetTexts {
    public string id;
    public string title;
    public string caption; //presented under the title/icon of the asset
    public string short_info; //tooltip information: dynamic short information that can change
    public string description; //information on the popup window
}

[Serializable]
public class Asset: AssetTexts {
    public string baseAssetID;
    public List<AssetType> types = new List<AssetType>();
    public OperationalState operationalState;
    public SecurityState securityState;
    public List<Capability> capabilities = new List<Capability>();
    public string icon;
    //Evidence evidence;
    public List<Action> actions = new List<Action>();
    public int layer;
    public List<string> connections = new List<string>();

    public Asset() {}

    public Asset(Asset a) {
    id = a.id;
    baseAssetID = a.baseAssetID;
    title = a.title;
    caption = a.caption;
    description = a.description;
    icon = a.icon;
    layer = a.layer;
    types = new List<AssetType>(a.types);
    capabilities = new List<Capability>(a.capabilities);
    connections = new List<string>(a.connections);
    actions = new List<Action>(a.actions);
    }//Asset
}//Class

[Serializable]
public class EnumTypesAndLabels {
    public string[] network_layer_labels;
    public string[] message_popups;
    public string[] operational_state;
    public string[] security_state;
    public string[] evidence_types;
    public string[] report_audiences;
    public string[] report_types;
    public string[] report_tones;
    public string[] report_detail_levels;
    public string[] report_technicalities;
    public string[] report_scopes;
    public string[] report_creation_popups;
}

[Serializable]
public class EvidenceTexts {
    public string id;
    public string title;
    public string summaryText;
    public string detailedText;
    public string reportText;
}

[Serializable]
public class Evidence : EvidenceTexts {
    public string name;
    //public List<string> types = new List<string>(); //public EvidenceType type;
    public List<EvidenceType> evidenceTypes = new List<EvidenceType>(); //public EvidenceType type;
    public int stepRoundIndex = -1;
    public List<string> relatedEvidenceIds = new List<string>();
    public  List<string> tags = new List<string>();
    public string asset_id;
    public string source_asset;
    public string date_time;
    public string action_id;

    public Evidence() { }

    public Evidence(Evidence evi) {
        id = evi.id;
        name = evi.name;
        //types = evi.types;
        evidenceTypes = evi.evidenceTypes;
        title = evi.title;
        summaryText = evi.summaryText;
        detailedText = evi.detailedText;
        stepRoundIndex = evi.stepRoundIndex;
        relatedEvidenceIds = evi.relatedEvidenceIds;
        tags = evi.tags;
        asset_id = evi.asset_id;
        source_asset = evi.source_asset;
        date_time = evi.date_time;
        action_id = evi.action_id;
    }//constructor
}

[Serializable]
public class Goal {
    public string id;
    public GoalType type;
    public string baseGoalID;
    public int round;
    public string subjectID;
    public AssetType subjectType;
    public Property property;
    public string objectID;
    public List<AssetType> objectTypes = new List<AssetType>();
    public Predicate objectPredicate;
    public string sourceID;
    public List<AssetType> sourceTypes = new List<AssetType>();
    public string targetID;
    public List<AssetType> targetTypes = new List<AssetType>();
    public Predicate targetPredicate;
    public List<SEF> sefs;
    public List<Trigger> triggers = new List<Trigger>();
    public bool achieved;
}

[Serializable]
public class InfoMessage {
    public string window_title;
    public string image;
    public ReportAudience NPC;
    public string title;
    public string text;
}

[Serializable]
public class PopupMessages {
    public string window_title_info;
    public string window_title_deadline_warning;
    public string window_title_no_time;
    public string window_title_no_aps;
    public string window_title_non_performable;
    public string window_title_session_completed;
    public string window_title_session_failed;
    public string window_title_warning_bi;
    public string text_session_completed;
    public string text_warning_bi;
    public string text_session_failed_bi;
    public string text_session_failed_ap;
    public string text_report_not_includes_elements;
    public string text_earned1ap;
    public string text_no_time;
    public string text_no_aps;
    public string text_required_time;
    public string text_remaining_time;
    public string text_vulnerability_found;
    public string text_of_round_popup_title;
    public string text_of_round_popup_1;
    public string text_aps_title_of_round_popup;
    public string text_deadline_title_of_round_popup;
    public string text_on_tutorials_action;
    public string text_action_already_performed;
}

[Serializable]
public class Metrics {
    public int CS;
    public int BI;
    public int budget;
    public int ActionPoints;
    public int trust;
    public int startStepRound;
    public int day;
    public int hour;
    public int min;
    public int sec;
    public int totalSeconds;
    public float midRoundTimerThreshold;
    public int deadline;
}//Metrics

[System.Serializable]
public class PerformedActionRecord {
    public int stepRoundIndex;
    public string assetID;
    public string actionID;
    public string objectID;
    public int totalSecs;
    public RoundPhase roundPhase;
    public int budget;
    public int actionPoints;
    public int cs;
    public int bi;
    public int stakeholderTrust;

    public PerformedActionRecord(int s_i, string a_id, string c_id, string o_id, int ts, RoundPhase rp, int bud, int aps, int c, int b, int tr) {
        stepRoundIndex = s_i;
        assetID = a_id;
        actionID = c_id;
        objectID = o_id;
        totalSecs = ts;
        roundPhase = rp;
        budget = bud;
        actionPoints = aps;
        cs = c;
        bi = b;
        stakeholderTrust = tr;
    }
}//PlayedActionRecord

[Serializable]
public class ReportAsset : Asset {
    public static readonly string[] KnownAudiences;
    public static readonly string[] KnownTypes;
    public static readonly string[] KnownTones; //= { "Formal", "Friendly", "Neutral" };

    // Grouped reference values for Contents: DetailLevel/Technicality/AudienceScope are
    // mutually-exclusive picks within their own group, PrivacyDataIncluded/LegallyFramed are
    // standalone flags - either present in Contents or not. Contents itself stays a flat
    // List<string> rather than five separate fields, matching the Types/Tags convention.
    public static readonly string[] DetailLevelOptions; // = { "Abstract", "Detailed" };
    public static readonly string[] TechnicalityOptions; //= { "Technical", "Non-Technical" };
    public static readonly string[] ScopeOptions; //= { "Internal", "External", "Public" };
    public const string PrivacyDataIncluded = "Privacy data included";
    public const string LegallyFramed = "Legally Framed";

    //Το χρειαζόμαστε;
    //public string Name;
    //public string Type;
    public List<string> Contents = new List<string>();
    //public string Tone;
    //public List<string> Audiences = new List<string>();
    //public DateTime Created;
    //public TimeSpan Duration;

    public List<string> selectedEvidenceIds = new List<string>();

    public List<ReportAudience> audiences = new List<ReportAudience>();
    public ReportAudience audience;
    public ReportDetailLevel detailLevel;
    public ReportScope scope;
    public ReportTechnicality technicality;
    public ReportTone tone;
    public ReportType report_type;
    public bool privacy_data_included;
    public bool legally_framed;
    //public List<string> selectedEvidenceIDs = new List<string>();
    
    private const int BaseApCost = 2;
    public int ApCost;

    // Creation/send cost: a base 2 AP, +1 AP per cost-raising choice (detail level, technicality,
    // privacy, audience reach, legal framing, formal tone). Only ever reads Contents/Tone, so it
    // stays accurate no matter when it's called as the user changes selections.
    public int CalculateApCost() {
        int cost = BaseApCost;
        if (detailLevel == ReportDetailLevel.DETAILED)
            cost += 1;

        if (technicality == ReportTechnicality.TECHNICAL)
            cost += 1;

        if (privacy_data_included)
            cost += 1;

        if (scope == ReportScope.EXTERNAL)
            cost += 1;

        if (scope == ReportScope.PUBLIC)
            cost += 1;

        if (legally_framed)
            cost += 1;

        if (tone == ReportTone.FORMAL)
            cost += 1;

        return cost;
    }//CalculateApCost

    static ReportAsset() {
        //------- ------- ------- ------- ------- ------- -------
        ReportAudience[] values_ra = (ReportAudience[])Enum.GetValues(typeof(ReportAudience));
        List<string> result = new List<string>();
        result.Add("");
        for (int i = 0; i < values_ra.Length; i++) {
            if (values_ra[i] == ReportAudience.UNDEFINED) {
                continue;
            }
            result.Add(values_ra[i].ToString());
        }
        KnownAudiences = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        ReportType[] values_rt = (ReportType[])Enum.GetValues(typeof(ReportType));
        result.Clear();
        result.Add("");
        for (int i = 0; i < values_rt.Length; i++) {
            if (values_rt[i] == ReportType.UNDEFINED) {
                continue;
            }
            result.Add(values_rt[i].ToString());
        }
        KnownTypes = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        ReportTone[] values_rto = (ReportTone[])Enum.GetValues(typeof(ReportTone));
        result.Clear();
        result.Add("");
        for (int i = 0; i < values_rto.Length; i++) {
            if (values_rto[i] == ReportTone.UNDEFINED) {
                continue;
            }
            result.Add(values_rto[i].ToString());
        }
        KnownTones = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        ReportDetailLevel[] values_rdl = (ReportDetailLevel[])Enum.GetValues(typeof(ReportDetailLevel));
        result.Clear();
        result.Add("");
        for (int i = 0; i < values_rdl.Length; i++) {
            if (values_rdl[i] == ReportDetailLevel.UNDEFINED) {
                continue;
            }
            result.Add(values_rdl[i].ToString());
        }
        DetailLevelOptions = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        ReportTechnicality[] values_rtech = (ReportTechnicality[])Enum.GetValues(typeof(ReportTechnicality));
        result.Clear();
        result.Add("");
        for (int i = 0; i < values_rtech.Length; i++) {
            if (values_rtech[i] == ReportTechnicality.UNDEFINED) {
                continue;
            }
            result.Add(values_rtech[i].ToString());
        }
        TechnicalityOptions = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        ReportScope[] values_rs = (ReportScope[])Enum.GetValues(typeof(ReportScope));
        result.Clear();
        result.Add("");
        for (int i = 0; i < values_rs.Length; i++) {
            if (values_rs[i] == ReportScope.UNDEFINED) {
                continue;
            }
            result.Add(values_rs[i].ToString());
        }
        ScopeOptions = result.ToArray();
        //------- ------- ------- ------- ------- ------- -------
        //------- ------- ------- ------- ------- ------- -------
    }//public ReportAsset
}//public class

[Serializable]
public class ApTimeTrade
{
    public int secondsPerAp = 600;          // 1 AP = 10 minutes
    public int maxApPerRound = 5;           // net AP traded per round, either direction; 0 = unlimited
    public int minSecondsRemaining = 300;   // selling time can never leave less than this
}

[Serializable]
public class ScenarioData {
    public string id;
    public string title;
    public string language;
    public string version;
    public UITexts ui_texts;
    public EnumTypesAndLabels enum_types_labels;
    public bool autoStartNextRound;
    public ApTimeTrade ap_time_trade = new ApTimeTrade();
    public List<Asset> assets_store;
    public List<Action> actions_store;
    public List<Goal> goals;
    public List<StepRound> step_rounds = new List<StepRound>();
    public List<TimelinePoint> timeline_points = new List<TimelinePoint>();
    public List<WatchBoardItem> watch_board = new List<WatchBoardItem>();
}//ScenarioData

[Serializable]
public class ScenarioLanguageData {
    public string id;
    public string language;
    public string scenario_title;
    public UITexts ui_texts;
    public List<AssetTexts> assets_texts;
    public List<ActionTexts> actions_texts;
    public EnumTypesAndLabels enum_types_labels;
    public List<EvidenceTexts> evidence_texts;
    public PopupMessages popup_messages;
    public List<TriggerTexts> triggers_texts;
    public  List<WatchBoardItemTexts> watchboard_items_texts;
}//ScenarioLanguageData

[Serializable]
public class SEF {
    //The SEF is related with a specific goal, the relatedGoalType identifies the type of the related goal.
    public GoalType relatedGoalType = 0;
    //for a sequence of actions the learner has to perform use 'seq_of_actions_ids'
    public List<string> seq_of_actions_ids = new List<string>();
    //for a certain num of type of actions we want learner to perform use 'property'(_of_action) & 'num_of_actions'
    public Property property = 0;
    public int num_of_actions = 0;
    //for a certain num of evidence we want learner to discover use 'evidence_type' & 'num_of_evidence'
    public EvidenceType evidence_type;
    public int num_of_evidence = 0;
    public int threshold = 0;
    public List<ReportAudience> reportAudiences = new List<ReportAudience>();
    public ReportDetailLevel reportDetailLevel = 0;
    public ReportScope reportScope = 0;
    public ReportTechnicality reportTechnicality = 0;
    public ReportTone reportTone = 0;
    public ReportType reportType = 0;
    public bool privacyDataIncluded = false;
    public bool legallyFramed = false;
    public List<string> selectedEvidenceIDs = new List<string>();
}//SEF

[Serializable]
public class StepRound {
    public string id;
    public string label;
    public string title;
    public string description;
    public List<Goal> goals;
    public Metrics stepMetrics;
    public List<Goal> sustainable_goals;
    public List<Asset> assets = new List<Asset>();
}//StepRound

[Serializable]
public class TimelinePoint {
    public string id;
    public int time; //start time In Seconds Of TP
    public RoundPhase phase;
    public Trigger trigger;
    public bool lockTriggerPerformer;//Process the trigger alone and not in parallel with other triggers (e.g., of the same phase)
    public bool passed;

    public TimelinePoint () {
        phase = RoundPhase.UNDEFINED;
        time = -1;
        trigger = null;
        passed = false;
    }//constructor

    public TimelinePoint (RoundPhase p, int t, Trigger tr) {
        phase = p;
        time = t;
        trigger = tr;
        passed = false;
    }//constructor
}//TimelinePoint

[Serializable]
public class TriggerTexts {
     public string id;
     public string info;
}

[Serializable]
public class Trigger: TriggerTexts {
    public string subjectID;
    public TriggerType property;
    public string objectID;
    public string sourceID;
    public string targetID;
    public string parentAssetID; //if the trigger is related with an action (and not TLP or watch board item), this is the id of the action
    public string parentActionID; //if the trigger is related with an action (and not TLP or watch board item), this is the id of the action
    //--- RESULT ---
    public RepeatMode repeatMode;
    public RoundPhase triggered; //usually NOW or UNDEFINED. If UNDEFINED it will triggered according to StartInSecs and durationInSecs of the Action
    public List<Evidence> evidence;
    public string task;
    public string asset_id;
    public string action_id;
    public int cs_change;
    public int bi_change;
    public int budget_change;
    public int aps_change;
    public int stakeholder_change;
    public string watch_board_item_id;
    public int new_state;

    public Trigger(){}

    public Trigger (Trigger t) {
        id = t.id;
        subjectID = t.subjectID;
        property = t.property;
        objectID = t.objectID;
        sourceID = t.sourceID;
        targetID = t.targetID;
        repeatMode = t.repeatMode;
        triggered = t.triggered;
        info = t.info;
        evidence = t.evidence;
        task = t.task;
        asset_id = t.asset_id;
        action_id = t.action_id;
        cs_change = t.cs_change;
        bi_change = t.bi_change;
        budget_change = t.budget_change;
        aps_change = t.aps_change;
        stakeholder_change = t.stakeholder_change;
        watch_board_item_id = t.watch_board_item_id;
        new_state = t.new_state;
    }

}//Trigger

[Serializable]
public class UITexts {
    public string round_label;
    public string day_label;
    public string ap_title;
    public string cs_title;
    public string bi_title;
    public string budget_title;
    public string deadline_title;
    public string trust_title;
    public string popup_round_text1;
    public string popup_round_aps_title;
    public string popup_round_deadline_title;
    public string action_bar_label;
    public string restart_button;
    public string restart_saved_button;
    public string save_button;
    public string save_button_saving;
    public string save_button_saved;
    public string save_button_failed;
    public string save_button_unavailable;
    public string exit_button;
    public string trade_aps;
    public string confirm_restart_title;
    public string confirm_restart_text;
    public string confirm_yes;
    public string confirm_no;
    public string trade_title;
    public string trade_ap_label;
    public string trade_time_label;
    public string trade_limit_label;
    public string trade_spend_button;
    public string trade_gain_button;
    public string trade_close_button;
    public string scheduled_tasks_label;
    public string news_button;
    public string threats_button;
    public string network_button;
    public string report_builder_button;
    public string fit_view_button;
    public string report_audience_label;
    public string report_type_label;
    public string report_tone_label;
    public string report_detail_label;
    public string report_technicality_label;
    public string report_scope_label;
    public string report_privacy_label;
    public string report_legal_label;
    public string report_send_button;
    public string evidence_repo_label;
    public string to_report_button;
    public string search_textbox;
    public string type_dropdown;
    public string evidence_attach_button;
}//UITexts


[Serializable]
public class WatchBoardItemTexts {
    public string id;//μοναδικό id του item
    public string text;//το σύντομο κείμενο που εμφανίζεται στο WatchBoard
}//WatchBoardItemTextsß

[Serializable]
public class WatchBoardItem : WatchBoardItemTexts {
    public string name;//εσωτερικό/αναγνωριστικό όνομα
    public WatchBoardItemType type; // Task / News / Threat
    public string description;//extra πληροφορία αν θέλουμε να εμφανίζεται
    public string goalID;
    public Severity severity;// Info / Warning / Critical / Neutral
    public WatchBoardItemStatus status;//active / inactive / performed / expired / hidden
    public int createAt;//πότε δημιουργείται/εμφανίζεται
    public RoundPhase createAtPhase;//σε ποια φάση δημιουργείται/εμφανίζεται
    public int expiresAt;//πότε λήγει σε δευτερόλεπτα από την αρχή του session
    public RoundPhase expiresAtPhase;//σε ποια φάση δημιουργείται/εμφανίζεται
    public bool requiresAck;//αν ο learner πρέπει να κάνει acknowledge
    public bool hasReject;//αν το item μπορεί να απορριφθεί
    public bool hasCompletionAck;//αν το item μπορεί να δηλωθεί ως ολοκληρωμένο
    public bool reportItem;//αν μπορεί να δημιουργήσει item για Report Builder
    public List<Trigger> onCreationTriggers;//triggers όταν εμφανιστεί
    public List<Trigger> onAcknowledgeTriggers;//triggers όταν γίνει acknowledge
    public List<Trigger> onRejectTriggers;//triggers όταν γίνει reject
    public List<Trigger> onCompletionTriggers;//triggers όταν ολοκληρωθεί
    public List<Trigger> onExpireTriggers;//triggers όταν λήξει
    public bool presented;

    public WatchBoardItem(string i, string n, Property p, string t, string d, string gID, WatchBoardItemType ty, Severity s, WatchBoardItemStatus st, int cAt, RoundPhase cAtP, int eAt, RoundPhase eAtP, bool rAck, bool hRej, bool hCompAck, bool rItem) {
        id = i;
        name = n;
        type = ty;
        text = t;
        description = d;
        goalID = gID;
        severity = s;
        status = st;
        createAt = cAt;
        createAtPhase = cAtP;
        expiresAt = eAt;
        expiresAtPhase = eAtP;
        requiresAck = rAck;
        hasReject = hRej;
        hasCompletionAck = hCompAck;
        reportItem = rItem;
        onCreationTriggers = new List<Trigger>();
        onAcknowledgeTriggers = new List<Trigger>();
        onRejectTriggers = new List<Trigger>();
        onCompletionTriggers = new List<Trigger>();
        onExpireTriggers = new List<Trigger>();
        presented = false;
    }//constructor
}