using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo
{
    public class DbKeys
    {
        // TODO: Use GUIDs for ClientIDs 
        public static class ClientIds
        {
            public const string PcMac = "ff45a57a45174fdd9a22cb99173fae05";
            public const string Boca = "5a80791ecda3475e852f7d2e14fe279a";
            public const string Wemo = "61db6371787f45ce8d2bcb37c8efa810";
        }

        public static class ClientTenantKeys
        {
            public static string PcMac = "MyCompany";
            public static string Boca = "boca";
            public static string Wemo = "wemo";
        }

        public static class DirectoryIds
        {
            public static string PcMac = Identity.Config.IdentityConstants.CorpDirectoryId;
            public static string Boca = "a8d9c090533b4705be450920f3f2348a";
            public static string Wemo = "0b38bc3d005548519bb02a697f7080d1";
        }

        public static class PoolIds {
            public static string PcMacCorp = "2869827fa77e4e8eb7c90c7827c69287";
            public static string PcMacClient= "d0badb84ee8c4eb4a178c9c711125ada";
            public static string PcMacSite1 = "e837b22775eb4c0aac118af5169b47a8";
            public static string PcMacSite2 = "1bc1633cec1147feb00b476ab380d96d";
            public static string PcMacSite3 = "2f64fe26655e46e0a2e9e41c62e4d23a";
            public static string BocaClient = "c084dc8c417b44e08bd6ca4b1f12a7ee";
            public static string BocaDistrict = "ae90c10c363843ed947cec9badc1e2e2";
            public static string WemoClient = "b6af6ae946164060933a9ed43f46e7af";
            public static string WemoChurch = "ed876e76b5e843c7ae3e113da8ee0af2";
            public static string WemoKids = "74e9f56dd6734ff4ac235755de6a820f";
        }
  
        public static class UserIds
        {
            public static string Admin = "AFCF7980-4BA7-4DD2-879D-599D058F7E73";
            public static string AdminSupport = "3EW4GJHG-4BA7-4DD2-879D-5HJG8766G5E5";
            public static string AdminSupport2 = "A8DA53A7-59C1-4C64-B77E-E247A9BB044D";
            public static string AdminAccounting = "MO64AVJ8-4BA7-4DD2-879D-CF65C99MMBV2";
            public static string AdminAccounting2 = "76A8CF94-17DF-4B81-A4A2-1806C413536D";
            public static string AdminSales = "E28C51FD-CEBB-4DB0-852B-D0948052E12F";
            public static string AdminSales2 = "E74F3297-B8ED-4476-8A82-649927D8237E";
            public static string Rhonda = "D8979194-2EBD-4C43-830C-C034FCAD3FF7";
            public static string Jane = "EECEFCC1-8050-4A0F-A5A5-D7ED19A078A8";
            public static string Gabi = "0FEC6D4F-6D54-4F43-A7D5-DC158BF37AA9";
            public static string Dave = "A61C1365-D328-48E1-89A6-751E0F2731E1";
            public static string Michael = "76305E4F-8703-46E8-8AB2-B607DA336662";
            public static string Chris = "14607761-5809-45B0-8296-093C93FC1797";
            public static string Bill = "42B3B391-8342-45C3-8D58-AFFC1A878D03";
            public static string Greg = "49DB67EE-86EC-4053-AF60-05FBB5DFF3BA";
            public static string Keith = "E8450647-04F3-4A62-97DC-84953380D3C1";
            public static string Ricardo = "083BC9FA-E0B0-4AFB-860D-215A74913F7B";
            public static string Mandy = "F510AACE-254D-4AB6-AB59-6D6D799B1AB5";
            public static string Joel = "13B2D0D1-F8A6-487E-9D60-A1E89DCC610B";
            public static string Bobby = "9546ACA0-02B2-4EA3-B5A3-1A2B4F4871E0";
            public static string Candy = "BC012058-768A-4D33-922F-CFAD185CB4EC";
            public static string Steve = "D8B1D16D-BCEA-4A15-8955-5A447D1A8F05";
            public static string Sarah = "CA8094D6-4B46-4A8C-8604-DA21B1970577";
            public static string Sally = "8B742D95-DE0C-4033-8232-4FC489FA09E1";
            public static string Susan = "0B88192F-9251-475A-852A-CAC94A95373F";
            public static string Sammy = "5F5EE4C7-6049-4212-BBFF-C746926840C4";
            public static string Sherry = "C1C90316-9BDC-4AA2-B097-26D2ADB21F09";
            public static string Christie = "406EF537-2B70-4776-B190-14280AB54E3A";
            public static string Stephanie = "9AB2EFB9-9CCC-4361-8606-070B444DED4E";
            public static string JonathanG = "C5A0B4B6-D5AE-4EE1-A744-9A6146041D1E";
            public static string SophieJ = "DB85BE6E-3BC3-4267-9581-D4208687E8BD";
            public static string OliverB = "07E2BEB9-E8CB-4C51-86FB-D0B8B6C598FE";
            public static string YasminM = "5FA40721-AE12-41FE-BCEF-622031674596";
            public static string JohnnyB = "22DC1F96-10E7-452B-8487-30107134B4D0";
            public static string BillyB = "3F203676-C75D-42B1-9704-ABA0347A7BC7";
        }
      
        public static class SiteIds
        {
            public const string Boca_District = "b75cbabb839f4b20ba6eb74241080201";
            public const string Wemo_Church = "2e79f4bf1df7475f8d4bdfa6e0070992";
            public const string Wemo_Kids = "f2e5e2698db3460aa1792f3fcab86f8e";
            public const string PcMac_Site1 = "93fc6283ff1c45f9935d629f70d4fcfb";
            public const string PcMac_Site2 = "ddd8f0982e714a5ab47c975d6e212b73";
            public const string PcMac_Site3 = "fd5461dd9ef041aca019777c5b4050a0";
        }

        public static class SiteTenantKeys
        {
            public const string Boca_District = "boca-dist";
            public const string Wemo_Church = "wemo-faith";
            public const string Wemo_Kids = "wemo-kids";
            public const string PcMac_Site1 = "inspiration";
            public const string PcMac_Site2 = "guidance";
            public const string PcMac_Site3 = "cityscape";
        }

        public static class PageIds
        {
            public const string PcMac_PcMac1_Home = "8472a30cbad6400eae5c4c9c916b0800";
            public const string PcMac_PcMac2_Home = "ada174d01997461284c798909acfa29e";
            public const string PcMac_PcMac3_Home = "fb09b50feae84d18a41c57a7cd460af2";
            public const string Boca_District_Home = "4a085a54ee814cb4af5b3646ae1aa915";
            public const string Wemo_Church_Home = "322ad025f6f64684b44f52f2b6166e06";
            public const string Wemo_Kids_Home = "daee60a36d5e4c4194ea47eb0339e77e";
        }
       
        public static class SiteTemplateIds
        {
            public const string Empty = "empty";
            public const string Inspiration = "inspiration";
            public const string Guidance = "guidance";
            public const string CityScape = "cityscape";
            public const string Essential = "essential";
        }

        public static class ThemeIds
        {
            public const string Default = "default";
            public const string Hero = "hero";
        }

        public static class ClientProductAppIds
        {
            public const string PcMacApp1 = "69360192-7B70-4D23-8C18-9293112A1083";
            public const string BocaApp1 = "E9B9CF16-CC33-4743-86A4-F357998D65F0";
            public const string WemoApp1 = "0F2DDBE0-F196-418F-B944-A4A7730AEA99";
            public const string WemoApp2 = "77B46365-A6A9-43C8-AD63-06B6B612B564";
        }

        public static class ProductIds
        {
            public const string Internal = "product-0";
            public const string Silver = "product-1";
            public const string Gold = "product-2";
            public const string Platinum = "product-3";
            public const string Diamond = "product-4";
        }

        public static class ProductAddOnIds
        {
            public const string ExtraTemplate = "addon-1";
            public const string ExtraStorage = "addon-2";
        }

        public class ProductCategoryIds
        {
            public const string Angelo = "angelo";
            public const string Education = "edu";
            public const string Business = "biz";
            public const string Faith = "faith";
            public const string ChildCare = "kidz";
        }

        public static class RoleNames
        {
            // Core "Locked" Roles
            public static string CorpAdmins = "Corp Admins";
            public static string CorpSupport = "Corp Support";
            public static string CorpAccounting = "Corp Accounting";
            public static string ClientOnlyAdmins = "Client Only Admins";
            public static string ClientAdmins = "Community Admins"; //"Client Root Admins";
            public static string ClientUserAdmins = "Global User Admins";
            public static string ClientSiteAdmins = "Global Site Admins";
            public static string SiteAdmins = Angelo.Connect.Services.SitePublisher.ROLE_SITE_ADMIN;
            public static string SiteStaff = Angelo.Connect.Services.SitePublisher.ROLE_SITE_STAFF;
            public static string SiteUsers = Angelo.Connect.Services.SitePublisher.ROLE_SITE_USERS;
            public static string SiteUserAdmins = "Site User Admins";
            public static string SiteDesigners = "Site Designers";


            // Other Test Roles
            public static string Coaches = "Coaches";
            public static string Players = "Players";
            public static string Employees = "Employees";
            public static string Accounting = "Accounting";
            public static string Developers = "Development";
            public static string Sales = "Sales";
            public static string DistrictStaff = "District Staff";
            public static string OfficeStaff = "Office Staff";
            public static string Principles = "Principals";
            public static string Teachers = "Teachers";
            public static string Students = "Students";
            public static string Parents = "Parents";
        }

        public static class ClaimTypes
        {
            public static string ACL_Parent = "child-user";
            public static string Permission = "permission";
        }

        public static class WirelessProviderIds
        {
            public static string Verizon = "verizon";
            public static string Sprint = "sprint";
            public static string TMobile = "t-mobile";
            public static string ATT = "att";
            public static string Virgin = "virgin";
            public static string Trackfone = "tracfone";
            public static string MetroPCS = "metro-pcs";
            public static string BoostMobile = "boost-mobile";
            public static string Cricket = "cricket";
            public static string PTel = "ptel";
            public static string RepublicWireless = "republic-wireless";
            public static string GoogleFi = "google-fi";
            public static string Suncom = "suncom";
            public static string Ting = "ting";
            public static string USCellular = "us-cellular";
            public static string ConsumerCellular = "consumer-cellular";
            public static string CSpire = "c-spire";
            public static string PagePlus = "page-plus";
        }

    }
}
