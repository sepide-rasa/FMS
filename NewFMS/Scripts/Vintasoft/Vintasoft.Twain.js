// Copyright 2014-2018 VintaSoft Ltd. All rights reserved.
// This software is protected by International copyright laws.
// Any copying, duplication, deployment, redistribution, modification or other
// disposition hereof is STRICTLY PROHIBITED without an express written license
// granted by VintaSoft Ltd. This notice may not be removed or otherwise
// altered under any circumstances.
// This code may NOT be used apart of the VintaSoft product.
var Vintasoft;
(function(e){if(void 0==e.Shared)throw Error("Vintasoft.Shared is not found.");if("1.0.10.1"!==e.version)throw Error("Wrong version of Vintasoft.Shared script.");e.Twain=e.Twain||(e.Twain={});(function(c){c.version="10.2.8.1";var b=e.Shared;c.WebTwainDeviceCapabilityUsageJS=b.EnumGenerator.create([{name:"None",value:0},{name:"Get",value:1},{name:"GetDefault",value:2},{name:"GetCurrent",value:4}],!0);c.WebDeviceCapabilityIdJS=b.EnumGenerator.create([{name:"XferCount",value:1},{name:"ICompression",value:256},
{name:"IPixelType",value:257},{name:"IUnits",value:258},{name:"IXferMech",value:259},{name:"Author",value:4096},{name:"Caption",value:4097},{name:"FeederEnabled",value:4098},{name:"FeederLoaded",value:4099},{name:"TimeDate",value:4100},{name:"SupportedCaps",value:4101},{name:"ExtendedCaps",value:4102},{name:"AutoFeed",value:4103},{name:"ClearPage",value:4104},{name:"FeedPage",value:4105},{name:"RewindPage",value:4106},{name:"Indicators",value:4107},{name:"SupportedCapsExt",value:4108},{name:"PaperDetectable",
value:4109},{name:"UIControllable",value:4110},{name:"DeviceOnline",value:4111},{name:"AutoScan",value:4112},{name:"ThumbnailsEnabled",value:4113},{name:"Duplex",value:4114},{name:"DuplexEnabled",value:4115},{name:"EnableDsUiOnly",value:4116},{name:"CustomDsData",value:4117},{name:"EndOrSer",value:4118},{name:"JobControl",value:4119},{name:"Alarms",value:4120},{name:"AlarmVolume",value:4121},{name:"AutomaticCapture",value:4122},{name:"TimeBeforeFirstCapture",value:4123},{name:"TimeBetweenCaptures",
value:4124},{name:"ClearBuffers",value:4125},{name:"MaxBatchBuffers",value:4126},{name:"DeviceTimeDate",value:4127},{name:"PowerSupply",value:4128},{name:"CameraPreviewUI",value:4129},{name:"DeviceEvents",value:4130},{name:"SerialNumber",value:4132},{name:"Printer",value:4134},{name:"PrinterEnabled",value:4135},{name:"PrinterIndex",value:4136},{name:"PrinterMode",value:4137},{name:"PrinterString",value:4138},{name:"PrinterSuffix",value:4139},{name:"Language",value:4140},{name:"FeederAlignment",value:4141},
{name:"FeederOrder",value:4142},{name:"ReacquireAllowed",value:4144},{name:"BatteryMinutes",value:4146},{name:"BatteryPercentage",value:4147},{name:"CameraSide",value:4148},{name:"Segmented",value:4149},{name:"AutomaticSenseMedium",value:4155},{name:"CameraEnabled",value:4150},{name:"CameraOrder",value:4151},{name:"MicrEnabled",value:4152},{name:"FeederPrep",value:4153},{name:"FeederPocket",value:4154},{name:"CustomInterfaceGuid",value:4156},{name:"SupportedCapsSegmentUnique",value:4157},{name:"SupportedDats",
value:4158},{name:"DoubleFeedDetection",value:4159},{name:"DoubleFeedDetectionLength",value:4160},{name:"DoubleFeedDetectionSensitivity",value:4161},{name:"DoubleFeedDetectionResponse",value:4162},{name:"PaperHandling",value:4163},{name:"IndicatorsMode",value:4164},{name:"PrinterVerticalOffset",value:4165},{name:"PowerSaveTime",value:4166},{name:"ISupportedExtImageInfo",value:4446},{name:"IAutoBright",value:4352},{name:"IBrightness",value:4353},{name:"IContrast",value:4355},{name:"ICustomHalftone",
value:4356},{name:"IExposureTime",value:4357},{name:"IFilter",value:4358},{name:"IFlashUsed",value:4359},{name:"IGamma",value:4360},{name:"IHalftones",value:4361},{name:"IHighlight",value:4362},{name:"IImageFileFormat",value:4364},{name:"ILampState",value:4365},{name:"ILightSource",value:4366},{name:"IOrientation",value:4368},{name:"IPhysicalWidth",value:4369},{name:"IPhysicalHeight",value:4370},{name:"IShadow",value:4371},{name:"IFrames",value:4372},{name:"IXNativeResolution",value:4374},{name:"IYNativeResolution",
value:4375},{name:"IXResolution",value:4376},{name:"IYResolution",value:4377},{name:"IMaxFrames",value:4378},{name:"ITiles",value:4379},{name:"IBitOrder",value:4380},{name:"ICcittKFactor",value:4381},{name:"ILightPath",value:4382},{name:"IPixelFlavor",value:4383},{name:"IPlanarChunky",value:4384},{name:"IRotation",value:4385},{name:"ISupportedSizes",value:4386},{name:"IThreshold",value:4387},{name:"IXScaling",value:4388},{name:"IYScaling",value:4389},{name:"IBitOrderCodes",value:4390},{name:"IPixelFlavorCodes",
value:4391},{name:"IJpegPixelType",value:4392},{name:"ITimeFill",value:4394},{name:"IBitDepth",value:4395},{name:"IBitDepthReduction",value:4396},{name:"IUndefinedImageSize",value:4397},{name:"IImageDataSet",value:4398},{name:"IExtImageInfo",value:4399},{name:"IMinimumHeight",value:4400},{name:"IMinimumWidth",value:4401},{name:"IFlipRotation",value:4406},{name:"IBarCodeDetectionEnabled",value:4407},{name:"ISupportedBarCodeTypes",value:4408},{name:"IBarCodeMaxSearchPriorities",value:4409},{name:"IBarCodeSearchPriorities",
value:4410},{name:"IBarCodeSearchMode",value:4411},{name:"IBarCodeMaxRetries",value:4412},{name:"IBarCodeTimeout",value:4413},{name:"IZoomFactor",value:4414},{name:"IPatchCodeDetectionEnabled",value:4415},{name:"ISupportedPatchCodeTypes",value:4416},{name:"IPatchCodeMaxSearchPriorities",value:4417},{name:"IPatchCodeSearchPriorities",value:4418},{name:"IPatchCodeSearchMode",value:4419},{name:"IPatchCodeMaxRetries",value:4420},{name:"IPatchCodeTimeout",value:4421},{name:"IFlashUsed2",value:4422},{name:"IImageFilter",
value:4423},{name:"INoiseFilter",value:4424},{name:"IOverScan",value:4425},{name:"IAutomaticBorderDetection",value:4432},{name:"IAutomaticDeskew",value:4433},{name:"IAutomaticRotate",value:4434},{name:"IJpegQuality",value:4435},{name:"IFeederType",value:4436},{name:"IIccProfile",value:4437},{name:"IAutoSize",value:4438},{name:"IAutoDiscardBlankPages",value:4404},{name:"IAutomaticCropUsesFrame",value:4439},{name:"IAutomaticLengthDetection",value:4440},{name:"IAutomaticColorEnabled",value:4441},{name:"IAutomaticColorNonColorPixelType",
value:4442},{name:"IColorManagementEnabled",value:4443},{name:"IImageMerge",value:4444},{name:"IImageMergeHeightThreshold",value:4445},{name:"IFilmType",value:4447},{name:"IMirror",value:4448},{name:"IJpegSubsampling",value:4449},{name:"PrinterCharRotation",value:4167},{name:"PrinterFontStyle",value:4168},{name:"PrinterIndexLeadChar",value:4169},{name:"PrinterIndexMaxValue",value:4170},{name:"PrinterIndexNumDigits",value:4171},{name:"PrinterIndexStep",value:4172},{name:"PrinterIndexTrigger",value:4173},
{name:"PrinterStringPreview",value:4174}],!1);c.WebCountryCodeEnumJS=b.EnumGenerator.create([{name:"Afghanistan",value:1001},{name:"Algeria",value:213},{name:"Americansamoa",value:684},{name:"Andorra",value:33},{name:"Angola",value:1002},{name:"Anguilla",value:8090},{name:"Antigua",value:8091},{name:"Argentina",value:54},{name:"Aruba",value:297},{name:"Ascensioni",value:247},{name:"Australia",value:61},{name:"Austria",value:43},{name:"Bahamas",value:8092},{name:"Bahrain",value:973},{name:"Bangladesh",
value:880},{name:"Barbados",value:8093},{name:"Belgium",value:32},{name:"Belize",value:501},{name:"Benin",value:229},{name:"Bermuda",value:8094},{name:"Bhutan",value:1003},{name:"Bolivia",value:591},{name:"Botswana",value:267},{name:"Britain",value:6},{name:"Britvirginis",value:8095},{name:"Brazil",value:55},{name:"Brunei",value:673},{name:"Bulgaria",value:359},{name:"Burkinafaso",value:1004},{name:"Burma",value:1005},{name:"Burundi",value:1006},{name:"Camaroon",value:237},{name:"Canada",value:2},
{name:"Capeverdeis",value:238},{name:"Caymanis",value:8096},{name:"Centralafrep",value:1007},{name:"Chad",value:1008},{name:"Chile",value:56},{name:"China",value:86},{name:"Christmasis",value:1009},{name:"Cocosis",value:1009},{name:"Colombia",value:57},{name:"Comoros",value:1010},{name:"Congo",value:1011},{name:"Cookis",value:1012},{name:"Costarica",value:506},{name:"Cuba",value:5},{name:"Cyprus",value:357},{name:"Czechoslovakia",value:42},{name:"Denmark",value:45},{name:"Djibouti",value:1013},{name:"Dominica",
value:8097},{name:"Domincanrep",value:8098},{name:"Easteris",value:1014},{name:"Ecuador",value:593},{name:"Egypt",value:20},{name:"Elsalvador",value:503},{name:"Eqguinea",value:1015},{name:"Ethiopia",value:251},{name:"Falklandis",value:1016},{name:"Faeroeis",value:298},{name:"Fijiislands",value:679},{name:"Finland",value:358},{name:"France",value:33},{name:"Frantilles",value:596},{name:"Frguiana",value:594},{name:"Frpolyneisa",value:689},{name:"Futanais",value:1043},{name:"Gabon",value:241},{name:"Gambia",
value:220},{name:"Germany",value:49},{name:"Ghana",value:233},{name:"Gibralter",value:350},{name:"Greece",value:30},{name:"Greenland",value:299},{name:"Grenada",value:8099},{name:"Grenedines",value:8015},{name:"Guadeloupe",value:590},{name:"Guam",value:671},{name:"Guantanamobay",value:5399},{name:"Guatemala",value:502},{name:"Guinea",value:224},{name:"Guineabissau",value:1017},{name:"Guyana",value:592},{name:"Haiti",value:509},{name:"Honduras",value:504},{name:"Hongkong",value:852},{name:"Hungary",
value:36},{name:"Iceland",value:354},{name:"India",value:91},{name:"Indonesia",value:62},{name:"Iran",value:98},{name:"Iraq",value:964},{name:"Ireland",value:353},{name:"Israel",value:972},{name:"Italy",value:39},{name:"Ivorycoast",value:225},{name:"Jamaica",value:8010},{name:"Japan",value:81},{name:"Jordan",value:962},{name:"Kenya",value:254},{name:"Kiribati",value:1018},{name:"Korea",value:82},{name:"Kuwait",value:965},{name:"Laos",value:1019},{name:"Lebanon",value:1020},{name:"Liberia",value:231},
{name:"Libya",value:218},{name:"Liechtenstein",value:41},{name:"Luxenbourg",value:352},{name:"Macao",value:853},{name:"Madagascar",value:1021},{name:"Malawi",value:265},{name:"Malaysia",value:60},{name:"Maldives",value:960},{name:"Mali",value:1022},{name:"Malta",value:356},{name:"Marshallis",value:692},{name:"Mauritania",value:1023},{name:"Mauritius",value:230},{name:"Mexico",value:3},{name:"Micronesia",value:691},{name:"Miquelon",value:508},{name:"Monaco",value:33},{name:"Mongolia",value:1024},{name:"Montserrat",
value:8011},{name:"Morocco",value:212},{name:"Mozambique",value:1025},{name:"Namibia",value:264},{name:"Nauru",value:1026},{name:"Nepal",value:977},{name:"Netherlands",value:31},{name:"Nethantilles",value:599},{name:"Nevis",value:8012},{name:"Newcaledonia",value:687},{name:"Newzealand",value:64},{name:"Nicaragua",value:505},{name:"Niger",value:227},{name:"Nigeria",value:234},{name:"Niue",value:1027},{name:"Norfolki",value:1028},{name:"Norway",value:47},{name:"Oman",value:968},{name:"Pakistan",value:92},
{name:"Palau",value:1029},{name:"Panama",value:507},{name:"Paraguay",value:595},{name:"Peru",value:51},{name:"Phillippines",value:63},{name:"Pitcairnis",value:1030},{name:"Pnewguinea",value:675},{name:"Poland",value:48},{name:"Portugal",value:351},{name:"Qatar",value:974},{name:"Reunioni",value:1031},{name:"Romania",value:40},{name:"Rwanda",value:250},{name:"Saipan",value:670},{name:"Sanmarino",value:39},{name:"Saotome",value:1033},{name:"Saudiarabia",value:966},{name:"Senegal",value:221},{name:"Seychellesis",
value:1034},{name:"Sierraleone",value:1035},{name:"Singapore",value:65},{name:"Solomonis",value:1036},{name:"Somali",value:1037},{name:"Southafrica",value:27},{name:"Spain",value:34},{name:"Srilanka",value:94},{name:"Sthelena",value:1032},{name:"Stkitts",value:8013},{name:"Stlucia",value:8014},{name:"Stpierre",value:508},{name:"Stvincent",value:8015},{name:"Sudan",value:1038},{name:"Suriname",value:597},{name:"Swaziland",value:268},{name:"Sweden",value:46},{name:"Switzerland",value:41},{name:"Syria",
value:1039},{name:"Taiwan",value:886},{name:"Tanzania",value:255},{name:"Thailand",value:66},{name:"Tobago",value:8016},{name:"Togo",value:228},{name:"Tongais",value:676},{name:"Trinidad",value:8016},{name:"Tunisia",value:216},{name:"Turkey",value:90},{name:"Turkscaicos",value:8017},{name:"Tuvalu",value:1040},{name:"Uganda",value:256},{name:"Uaemirates",value:971},{name:"Unitedkingdom",value:44},{name:"Usa",value:1},{name:"Uruguay",value:598},{name:"Vanuatu",value:1041},{name:"Vaticancity",value:39},
{name:"Venezuela",value:58},{name:"Wake",value:1042},{name:"Wallisis",value:1043},{name:"Westernsahara",value:1044},{name:"Westernsamoa",value:1045},{name:"Yemen",value:1046},{name:"Yugoslavia",value:38},{name:"Zaire",value:243},{name:"Zambia",value:260},{name:"Zimbabwe",value:263},{name:"Albania",value:355},{name:"Armenia",value:374},{name:"Azerbaijan",value:994},{name:"Belarus",value:375},{name:"Bosniaherzgo",value:387},{name:"Cambodia",value:855},{name:"Croatia",value:385},{name:"Czechrepublic",
value:420},{name:"Diegogarcia",value:246},{name:"Eritrea",value:291},{name:"Estonia",value:372},{name:"Georgia",value:995},{name:"Latvia",value:371},{name:"Lesotho",value:266},{name:"Lithuania",value:370},{name:"Macedonia",value:389},{name:"Mayotteis",value:269},{name:"Moldova",value:373},{name:"Myanmar",value:95},{name:"Northkorea",value:850},{name:"Puertorico",value:787},{name:"Russia",value:7},{name:"Serbia",value:381},{name:"Slovakia",value:421},{name:"Slovenia",value:386},{name:"Southkorea",
value:82},{name:"Ukraine",value:380},{name:"Usvirginis",value:340},{name:"Vietnam",value:84}],!1);c.WebLanguageTypeEnumJS=b.EnumGenerator.create([{name:"UserLocale",value:-1},{name:"Danish",value:0},{name:"Dutch",value:1},{name:"English",value:2},{name:"FrenchCanadian",value:3},{name:"Finnish",value:4},{name:"French",value:5},{name:"German",value:6},{name:"Icelandic",value:7},{name:"Italian",value:8},{name:"Norwegian",value:9},{name:"Portuguese",value:10},{name:"Spanish",value:11},{name:"Swedish",
value:12},{name:"EnglishUsa",value:13},{name:"Afrikaans",value:14},{name:"Albania",value:15},{name:"Arabic",value:16},{name:"ArabicAlgeria",value:17},{name:"ArabicBahrain",value:18},{name:"ArabicEgypt",value:19},{name:"ArabicIraq",value:20},{name:"ArabicJordan",value:21},{name:"ArabicKuwait",value:22},{name:"ArabicLebanon",value:23},{name:"ArabicLibya",value:24},{name:"ArabicMorocco",value:25},{name:"ArabicOman",value:26},{name:"ArabicQatar",value:27},{name:"ArabicSaudiArabia",value:28},{name:"ArabicSyria",
value:29},{name:"ArabicTunisia",value:30},{name:"ArabicUae",value:31},{name:"ArabicYemen",value:32},{name:"Basque",value:33},{name:"Byelorussian",value:34},{name:"Bulgarian",value:35},{name:"Catalan",value:36},{name:"Chinese",value:37},{name:"ChineseHongkong",value:38},{name:"ChinesePrc",value:39},{name:"ChineseSingapore",value:40},{name:"ChineseSimplified",value:41},{name:"ChineseTaiwan",value:42},{name:"ChineseTraditional",value:43},{name:"Croatia",value:44},{name:"Czech",value:45},{name:"DutchBelgian",
value:46},{name:"EnglishAustralian",value:47},{name:"EnglishCanadian",value:48},{name:"EnglishIreland",value:49},{name:"EnglishNewZealand",value:50},{name:"EnglishSouthAfrica",value:51},{name:"EnglishUnitedKingdom",value:52},{name:"Estonian",value:53},{name:"Faeroese",value:54},{name:"Farsi",value:55},{name:"FrenchBelgian",value:56},{name:"FrenchLuxembourg",value:57},{name:"FrenchSwiss",value:58},{name:"GermanAustrian",value:59},{name:"GermanLuxembourg",value:60},{name:"GermanLiechtenstein",value:61},
{name:"GermanSwiss",value:62},{name:"Greek",value:63},{name:"Hebrew",value:64},{name:"Hungarian",value:65},{name:"Indonesian",value:66},{name:"ItalianSwiss",value:67},{name:"Japanese",value:68},{name:"Korean",value:69},{name:"KoreanJihab",value:70},{name:"Latvian",value:71},{name:"Lithuanian",value:72},{name:"NorwegianBokmal",value:73},{name:"NorwegianNynorsk",value:74},{name:"Polish",value:75},{name:"PortugueseBrazil",value:76},{name:"Romanian",value:77},{name:"Russian",value:78},{name:"SerbianLatin",
value:79},{name:"Slovak",value:80},{name:"Slovenian",value:81},{name:"SpanishMexican",value:82},{name:"SpanishModern",value:83},{name:"Thai",value:84},{name:"Turkish",value:85},{name:"Ukrainian",value:86},{name:"Assmese",value:87},{name:"Bengali",value:88},{name:"Bihari",value:89},{name:"Bodo",value:90},{name:"Dogri",value:91},{name:"Gujarati",value:92},{name:"Haryanvi",value:93},{name:"Hindi",value:94},{name:"Kannada",value:95},{name:"Kashmiri",value:96},{name:"Malayalam",value:97},{name:"Marathi",
value:98},{name:"Marwari",value:99},{name:"Meghalayan",value:100},{name:"Mizo",value:101},{name:"Naga",value:102},{name:"Orissi",value:103},{name:"Punjabi",value:104},{name:"Pushtu",value:105},{name:"SerbianCyrillic",value:106},{name:"Sikkimi",value:107},{name:"SwedishFinland",value:108},{name:"Tamil",value:109},{name:"Telugu",value:110},{name:"Tripuri",value:111},{name:"Urdu",value:112},{name:"Vietnamese",value:113}],!1);var m=function(){var a=m.prototype;a.get_IsTwain2Compatible=function(){return this._410};
a.set_IsTwain2Compatible=function(a){b.pv.b(a);this._410=a};a.get_CountryCode=function(){return this._330};a.set_CountryCode=function(a){this._330=b.pv.e(a,c.WebCountryCodeEnumJS)};a.get_LanguageType=function(){return this._136};a.set_LanguageType=function(a){b.pv.e(a,c.WebLanguageTypeEnumJS);this._136=a};a.get_ApplicationProductName=function(){return this._391};a.set_ApplicationProductName=function(a){b.pv.s(a);this._391=a};a.get_TopMostUiWindow=function(){return this._21};a.set_TopMostUiWindow=
function(a){b.pv.b(a);this._21=a};a.get_AllowableSessionInactivityTime=function(){return this._77};a.set_AllowableSessionInactivityTime=function(a){b.pv.n(a);(100>a||36E5<a)&&pv.toe();this._77=a};this._410=!0;this._330=new c.WebCountryCodeEnumJS(1);this._136=new c.WebLanguageTypeEnumJS(2);this._391="VintaSoft Twain";this._21=!0;this._77=6E4},l=function(c){function e(){var a=new b.WebRequestJS("RefreshTwainSession",void 0,void 0,{type:"POST",data:{twainSessionId:p._174}});p._112.sendRequest(a)}b.pv.c(c,
b.WebServiceJS);this._112=c;this._174;this._212;this._258;this._20=void 0;this._410=this._222=!1;var p=this;l.prototype.get_IsOpened=function(){return this._222};l.prototype.get_IsTwain2Compatible=function(){return this._410};l.prototype.get_OpenedDevice=function(){return this._20};l.prototype.get_TwainSessionID=function(){return this._174};l.prototype.open=function(a,c,g){"function"===typeof a&&(g=c,c=a,a=new m);b.pv.c(a,m);var f=this,k=new b.WebRequestJS("CreateTwainSession",function(k,d,s){f._174=
k.sessionId;f._212=setInterval(e,a._77);k=new b.WebRequestJS("OpenDeviceManager",function(k,b,h){f._222=!0;f._410=a._410;void 0!=c&&c(k,b,h)},g,{type:"POST",data:{twainSessionId:f._174,isTwain2Compatible:a._410,applicationProductName:a._391,topMostUiWindow:a._21}});return f._112.sendRequest(k)},g,{type:"POST",data:{countryCode:a._330.valueOf(),languageType:a._136.valueOf(),applicationUrl:window.location.href}});return this._112.sendRequest(k)};l.prototype.close=function(a,c){!1===this._222&&b.pv.te("Device manager is not open.");
var g=this,f=new b.WebRequestJS("CloseTwainSession",function(k,b,d){g._222=!1;g._174=void 0;g._258=void 0;g._20=void 0;void 0!=a&&a(k,b,d)},c,{type:"POST",data:{twainSessionId:this._174}});void 0!=this._212&&(clearInterval(this._212),this._212=void 0);return this._112.sendRequest(f)};l.prototype.getDevices=function(a,b){if(void 0==this._258)this.refreshDevices(a,b);else return this._258};l.prototype.refreshDevices=function(c,e){var g=this,f=new b.WebRequestJS("GetDeviceInfos",function(k,b,d){var f=
k.devices;g._258=[];for(var e=0;e<f.length;e++){var n=f[e],n=new a(n.productName,n.productFamily,n.manufacturer,n.driverVersion,n.twainVersion,g);g._258.push(n)}k={devices:g._258,defaultDeviceIndex:k.defaultDeviceIndex};void 0!=c&&c(k,b,d)},e,{type:"POST",data:{twainSessionId:this._174}});this._112.sendRequest(f)}},a=function(e,m,p,q,r,g){function f(a){a._268._20==a&&a._222||b.pv.te("Device is not opened.")}b.pv.c(g,l);b.pv.s(e,m,p,q,r);this._268=g;this._222=!1;this._17=e;this._389=m;this._166=p;
this._431=q;this._59=r;a.prototype.get_IsOpened=function(){return this._222};a.prototype.get_DeviceName=function(){return this._17};a.prototype.get_ProductFamily=function(){return this._389};a.prototype.get_Manufacturer=function(){return this._166};a.prototype.get_DeviceName=function(){return this._17};a.prototype.get_DriverVersion=function(){return this._431};a.prototype.get_TwainVersion=function(){return this._59};a.prototype.get_IsTwain2Compatible=function(){return 2<=Number(this._59)};a.prototype.get_IsWIA=
function(){return this._17.startsWith?this._17.startsWith("WIA - "):0===this._17.search("WIA - ")};a.prototype.open=function(a,h,d){void 0!=this._268._20&&b.pv.te("Another device '"+this._268._20.get_DeviceName()+"' is opened already.");b.pv.b(a);var c=this;a=new b.WebRequestJS("OpenDevice",function(a,k,b){c._268._20=c;c._222=!0;void 0!=h&&h(a,k,b)},d,{type:"POST",data:{deviceName:this._17,twainSessionId:this._268._174,showUI:a}});return this._268._112.sendRequest(a)};a.prototype.close=function(a,
c){f(this);var d=this,e=new b.WebRequestJS("CloseDevice",function(b,c,h){d._268._20=void 0;d._222=!1;void 0!=a&&a(b,c,h)},c,{type:"POST",data:{twainSessionId:this._268._174}});return this._268._112.sendRequest(e)};a.prototype.acquireModal=function(a,c){f(this);var d=new b.WebRequestJS("AcquireModal",a,c,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17}});return this._268._112.sendRequest(d)};a.prototype.cancelTransfer=function(a,c){f(this);var d=new b.WebRequestJS("CancelTransfer",
a,c,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17}});return this._268._112.sendRequest(d)};a.prototype.getSupportedCapabilities=function(a,h,d){f(this);a=b.pv.e(a,c.WebTwainDeviceCapabilityUsageJS);a=new b.WebRequestJS("GetSupportedDeviceCapabilities",h,d,{type:"POST",data:{twainSessionId:this._268._174,usageMode:a.valueOf(),deviceName:this._17}});return this._268._112.sendRequest(a)};a.prototype.getCapability=function(a,h,d,e){f(this);h=b.pv.e(h,c.WebTwainDeviceCapabilityUsageJS);
a=a instanceof b.WebEnumItemBase?a.valueOf():a;b.pv.n(a);a=new b.WebRequestJS("GetDeviceCapability",d,e,{type:"POST",data:{twainSessionId:this._268._174,capabilityId:a,usageMode:h.valueOf(),deviceName:this._17}});return this._268._112.sendRequest(a)};a.prototype.getCapabilities=function(a,h,d,e){f(this);h=b.pv.e(h,c.WebTwainDeviceCapabilityUsageJS);var g=[];b.pv.ad(a,function(a){a=a instanceof b.WebEnumItemBase?a.valueOf():a;b.pv.n(a);g.push(a)});a=new b.WebRequestJS("GetDeviceCapabilities",d,e,{type:"POST",
data:{twainSessionId:this._268._174,capabilitiesId:g,usageMode:h.valueOf(),deviceName:this._17}});return this._268._112.sendRequest(a)};a.prototype.setCapability=function(a,c,d,e){f(this);a=a instanceof b.WebEnumItemBase?a.valueOf():a;b.pv.n(a);a=new b.WebRequestJS("SetDeviceCapability",d,e,{type:"POST",data:{twainSessionId:this._268._174,capabilityId:a,capabilityValue:c,deviceName:this._17}});return this._268._112.sendRequest(a)};a.prototype.getDefaultImageLayout=function(a,c){f(this);var d=new b.WebRequestJS("GetDefaultDeviceImageLayout",
a,c,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17,documentNumber:1,frameNumber:1,pageNumber:1}});return this._268._112.sendRequest(d)};a.prototype.getImageLayout=function(a,c){f(this);var d=new b.WebRequestJS("GetDeviceImageLayout",a,c,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17,documentNumber:1,frameNumber:1,pageNumber:1}});return this._268._112.sendRequest(d)};a.prototype.resetImageLayout=function(a,c){f(this);var d=new b.WebRequestJS("ResetDeviceImageLayout",
a,c,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17,documentNumber:1,frameNumber:1,pageNumber:1}});return this._268._112.sendRequest(d)};a.prototype.setImageLayout=function(a,c,d){f(this);b.pv.r(a);a=new b.WebRequestJS("SetDeviceImageLayout",c,d,{type:"POST",data:{twainSessionId:this._268._174,deviceName:this._17,documentNumber:1,frameNumber:1,pageNumber:1,frame:a}});return this._268._112.sendRequest(a)}};c.WebTwainDeviceManagerInitSettingsJS=m;c.WebTwainDeviceManagerJS=l;c.WebTwainDeviceJS=
a})(e.Twain||(e.Twain={}))})(Vintasoft||(Vintasoft={}));