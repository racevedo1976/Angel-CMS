using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Data;
using Angelo.Connect.Widgets.Models;
using Angelo.Plugins;

namespace Angelo.Connect.CoreWidgets.Data
{
    public class SeedJsonWidgets : IPluginStartupAction
    {
        private ConnectDbContext _dbContext;

        public SeedJsonWidgets(ConnectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Invoke()
        {
            if (_dbContext.JsonWidgets.Count() == 0)
            {
                #region Titles

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Title
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Title_DemoTitle1,
                            Text = "Demo Site"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Title
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Title_DemoTitle2,
                            Text = "Site Title"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Title
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Title_SchoolTitle,
                            Text = "School Name"
                        }
                    )
                );

                #endregion

                #region Alerts

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Alert
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Alert_DemoAlert,
                            Style = "alert-info",
                            Text = "This alert needs your attention, it's an important sample alert."
                        }
                    )
                );

                #endregion

                #region Images

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_Google,
                            Caption = "Google",
                            Src = "/img/SeedImages/Google.png"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoLogo1,
                            Caption = "Demo Logo",
                            Src = "/img/SeedImages/SisLogo.png",
                            Height = "130px",
                            Width = "130px"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoLogo2,
                            Caption = "Demo Logo",
                            Src = "/img/SeedImages/Joomlahunt.png",
                            Height = "130px",
                            Width = "100px"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_SchoolLogo,
                            Caption = "School Logo",
                            Src = "/img/SeedImages/HillsboroHelmet.png",
                            Height = "130px",
                            Width = "130px"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImage1,
                            Caption = "Demo Image 1",
                            Src = "/img/SeedImages/technics1_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImage2,
                            Caption = "Demo Image 2",
                            Src = "/img/SeedImages/technics4_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImage3,
                            Caption = "Demo Image 3",
                            Src = "/img/SeedImages/technics5_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                  new JsonWidget(
                      new Models.Image
                      {
                          Id = HtmlDbSeedConstants.WidgetId_Image_DemoImageA,
                          Caption = "Demo Image A",
                          Src = "/img/SeedImages/sports1_400_200.jpg"
                      }
                  )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImageB,
                            Caption = "Demo Image B",
                            Src = "/img/SeedImages/sports2_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImageC,
                            Caption = "Demo Image C",
                            Src = "/img/SeedImages/sports3_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImageD,
                            Caption = "Demo Image D",
                            Src = "/img/SeedImages/people1_400_200.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_DemoImageE,
                            Caption = "Demo Image E",
                            Src = "/img/SeedImages/people2_400_200.jpg"
                        }
                    )
                );


                // inspriration site images
                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImageMain,
                            Caption = "School Main Image",
                            Src = "/templates/inspiration/images/slideshow1.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage1,
                            Caption = "School Image 1",
                            Src = "/templates/inspiration/images/1.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage2,
                            Caption = "School Image 2",
                            Src = "/templates/inspiration/images/2.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage3,
                            Caption = "School Image 3",
                            Src = "/templates/inspiration/images/3.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage4,
                            Caption = "School Image 4",
                            Src = "/templates/inspiration/images/4.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage5,
                            Caption = "School Round Image 1",
                            Src = "/templates/inspiration/images/button1.png"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage6,
                            Caption = "School Round Image 2",
                            Src = "/templates/inspiration/images/button2.png"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage7,
                            Caption = "School Round Image 3",
                            Src = "/templates/inspiration/images/button3.png"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_InspirationImage8,
                            Caption = "School Round Image 4",
                            Src = "/templates/inspiration/images/button4.png"
                        }
                    )
                );


                // guidance site images
                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_GuidanceImageMain,
                            Caption = "School Main Image",
                            Src = "/templates/guidance/images/home-main.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_GuidanceImage1,
                            Caption = "School Image 1",
                            Src = "/templates/guidance/images/home-1.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_GuidanceImage2,
                            Caption = "School Image 2",
                            Src = "/templates/guidance/images/home-2.jpg"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.Image
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Image_GuidanceImage3,
                            Caption = "School Image 3",
                            Src = "/templates/guidance/images/home-3.jpg"
                        }
                    )
                );

                #endregion

                #region Simple Slide Show
                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.SlideShow
                        {
                            Id = HtmlDbSeedConstants.WidgetId_SlideShow_Business,
                            Speed = 4000,
                            Image1Caption = "Competence",
                            Image1Src = "/img/SeedImages/business2_600_400.jpg",
                            Image2Caption = "Leadership",
                            Image2Src = "/img/SeedImages/business3_600_400.jpg",
                            Image3Caption = "Teamwork",
                            Image3Src = "/img/SeedImages/business4_600_400.jpg",
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.SlideShow
                        {
                            Id = HtmlDbSeedConstants.WidgetId_SlideShow_Nature,
                            Speed = 4000,
                            Image1Caption = "Ants aligned on a blade of grass",
                            Image1Src = "/img/SeedImages/nature2_600_400.jpg",
                            Image2Caption = "Coastal mist in the early morning",
                            Image2Src = "/img/SeedImages/nature3_600_400.jpg",
                            Image3Caption = "Sun setting along the coast",
                            Image3Src = "/img/SeedImages/nature5_600_400.jpg",
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.SlideShow
                        {
                            Id = HtmlDbSeedConstants.WidgetId_SlideShow_Animals,
                            Speed = 4000,
                            Image1Caption = "Adult male mountain gorilla",
                            Image1Src = "/img/SeedImages/animals2_600_400.jpg",
                            Image2Caption = "Tiger resting on a ledge",
                            Image2Src = "/img/SeedImages/animals3_600_400.jpg",
                            Image3Caption = "Baby rhino and mother",
                            Image3Src = "/img/SeedImages/animals1_600_400.jpg",
                        }
                    )
                );

                #endregion

                #region Hero Units

                _dbContext.JsonWidgets.Add(new JsonWidget(
                    new Models.HeroUnit
                    {
                        Id = HtmlDbSeedConstants.WidgetId_Hero_InspirationWelcome,
                        Title = "Welcome to Inspiration School District",
                        Body = "The mission of the Inspiration School District is to provide a child-centered environment that cultivates character, fosters academic excellence, and embraces diversity. District families, community, and staff join as partners to develop creative, exemplary learners with the skills and enthusiasm to contribute to a constantly changing global society."
                    }
                ));

                _dbContext.JsonWidgets.Add(new JsonWidget(
                    new Models.HeroUnit
                    {
                        Id = HtmlDbSeedConstants.WidgetId_Hero_GuidanceWelcome,
                        Title = "Welcome to Guidance High School",
                        Body = "The mission of our school is to provide a child-centered environment that cultivates character, fosters academic excellence, and embraces diversity. District families, community, and staff join as partners to develop creative, exemplary learners with the skills and enthusiasm to contribute to a constantly changing global society."
                    }
                ));

                #endregion

                #region Raw Html

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_BaconIpsum,
                            Html = "Cillum beef ribs excepteur commodo corned beef esse sunt. Corned beef quis ut bacon anim, excepteur pancetta t-bone strip steak incididunt exercitation landjaeger jowl. Picanha short loin in, cillum laboris bacon sint tri-tip qui laborum tongue frankfurter do pastrami. Ground round laborum brisket, enim nostrud consectetur ribeye ut shoulder pancetta velit leberkas sirloin labore salami. Venison mollit adipisicing corned beef dolore."
                        }
                    )
                );


                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_LoremIpsum1,
                            Html = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_LoremIpsum2,
                            Html = "Nobis virtute elaboraret qui et. Iuvaret intellegam vituperatoribus at pro, ei malorum placerat appellantur nam. No mel epicuri senserit, ex labitur perpetua laboramus mel. Sensibus aliquando intellegam cu vis, ei pro amet accusata concludaturque, eam purto insolens no. Sed elit feugiat signiferumque ut."
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_LoremIpsum3,
                            Html = "Dissentiet appellantur ad vis. Eros summo per et, per ne summo dolore definitiones. Et hinc corpora principes ius, id vis omnesque tractatos efficiendi. Cu epicuri fierent vis. Ex esse albucius vim, sale dolore ullamcorper pro et. Quis utamur aliquip mel in, mundi noster putant usu ea, an mea wisi voluptatibus."
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_LoremIpsum_Long,
                            Html = "Lorem ipsum dolor sit amet, bonorum assentior temporibus qui eu. Ferri facete est cu, an erant quaeque duo. In delectus atomorum qui, saepe inciderint ne eam. Possim labitur ad vix. Eam ad dicat oporteat tincidunt.<br/><br/>In sed aeque efficiantur, mel ei facilis perpetua. Per meliore facilisi invenire ei, ut mei quando oporteat imperdiet. Minim theophrastus ut qui, at per zril salutatus definitionem. Te vel graecis inciderint. Usu te propriae accusata expetenda, sed ex elitr tantas comprehensam. Et vix wisi tation timeam.<br/><br/>Ad vim illud volumus. Ad aperiri maiestatis reformidans ius, legere salutandi intellegat an cum. Dolorem iudicabit repudiare eam at, appellantur efficiantur per ea. Vis magna comprehensam et, pro te duis fuisset, qui laoreet delectus conclusionemque ne. Nobis libris atomorum an vim. Wisi virtute an eam, tation eruditi intellegam quo no.<br/><br/>Nobis virtute elaboraret qui et. Iuvaret intellegam vituperatoribus at pro, ei malorum placerat appellantur nam. No mel epicuri senserit, ex labitur perpetua laboramus mel. Sensibus aliquando intellegam cu vis, ei pro amet accusata concludaturque, eam purto insolens no. Sed elit feugiat signiferumque ut.<br/><br/>Maluisset laboramus eloquentiam mei ad, ad enim aliquam persecuti mel. Per ne detraxit deterruisset, illud velit iudicabit mel an, cum at tation oportere. No clita volumus nam, iusto scaevola vel ex, id vix timeam dolorem omittantur. Semper nonumes eam ea, mel alii tractatos ad, eos fugit etiam ei. Amet facilisi his in. In vel alterum inimicus incorrupte, numquam recusabo duo at.<br/><br/>Dissentiet appellantur ad vis. Eros summo per et, per ne summo dolore definitiones. Et hinc corpora principes ius, id vis omnesque tractatos efficiendi. Cu epicuri fierent vis. Ex esse albucius vim, sale dolore ullamcorper pro et. Quis utamur aliquip mel in, mundi noster putant usu ea, an mea wisi voluptatibus.<br/><br/>Dicit omittam cu sed, ad harum maiorum mel. Nostrud docendi verterem ei mei, nam utamur docendi sapientem no, aliquip utroque fastidii te nam. Doming deserunt interesset ne pro, ius mucius facilis ex. Putent gloriatur sit ex, ponderum invenire eos et.<br/><br/>Pri ei quas omnium. Melius diceret legendos sed at. Vocent quaestio platonem sit an, mea cibo ferri disputationi cu, sea et nihil quaestio inciderint. Oportere interesset quo no. Sea audire expetendis an, vel diam unum utamur ne.<br/><br/>At eius lucilius facilisis mea, eum at cibo accumsan maiestatis, eam audiam equidem definitionem ut. No vis agam tota falli. Cu est placerat postulant, sea cu debet aeque, no exerci consul mea. Eam labitur accusam suscipit ex, eum cu natum probo reprimique.<br/><br/>Quo autem inani doctus in. Vis blandit ponderum cu, ei mnesarchum cotidieque referrentur eam, suas quodsi pro ex. Option dignissim vituperata quo ut. Ex hinc tibique vix, ex mea recteque salutandi. Eum et alia viris, usu munere deserunt argumentum ex. Sumo pertinax honestatis cum no, eum et vitae pericula intellegat, vix facer omnium sapientem ad."
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_NavTop1,
                            Html = "<div><nav class=\"navbar navbar-default\"><div class=\"navbar-header\"><button class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#navbar1\"><span class=\"icon-bar\"></span><span class=\"icon-bar\"></span><span class=\"icon-bar\"></span></button></div><div class=\"navbar-collapse collapse\" id=\"navbar1\"><ul class=\"nav navbar-nav\"><li><a href=\"/\">Home</a></li> <li><a href=\"/about\">About</a></li><li><a href=\"/contact\">Contact</a></li></ul></div></nav></div>"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_NavTop2,
                            Html = "<div><nav class=\"navbar navbar-default\"><div class=\"navbar-header\"><button class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#navbar1\"><span class=\"icon-bar\"></span><span class=\"icon-bar\"></span><span class=\"icon-bar\"></span></button></div><div class=\"navbar-collapse collapse\" id=\"navbar1\"><ul class=\"nav navbar-nav\"><li><a href=\"/\">Home</a></li> <li><a href=\"#\">Link</a></li><li><a href=\"#\">Link</a></li></ul></div></nav></div>"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_NavLeft,
                            Html = "<ul class=\"list-group\"><li class=\"list-group-item\"><a href=\"#\">Link 1</a></li><li class=\"list-group-item\"><a href=\"#\">Link 2</a></li><li class=\"list-group-item\"><a href=\"#\">Link 3</a></li><li class=\"list-group-item\"><a href=\"#\">Link 4</a></li><li class=\"list-group-item\"><a href=\"#\">Link 5</a></li><li class=\"list-group-item\"><a href=\"#\">Link 6</a></li><li class=\"list-group-item\"><a href=\"#\">Link 7</a></li><li class=\"list-group-item\"><a href=\"#\">Link 8</a></li><li class=\"list-group-item\"><a href=\"#\">Link 9</a></li><li class=\"list-group-item\"><a href=\"#\">Link 10</a></li><li class=\"list-group-item\"><a href=\"#\">Link 11</a></li><li class=\"list-group-item\"><a href=\"#\">Link 12</a></li></ul>"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_Jumbotron,
                            Html = "<div class=\"jumbotron\"><h1>Hello, world!</h1><p>This is a simple hero unit, a simple jumbotron-style component for calling extra attention to featured content or information.</p><p><a class=\"btn btn-primary btn-lg\" href=\"#\" role=\"button\">Learn more</a></p></div>"
                        }
                    )
                );

                _dbContext.JsonWidgets.Add(
                    new JsonWidget(
                        new Models.RawHtml
                        {
                            Id = HtmlDbSeedConstants.WidgetId_Html_SiteSearch,
                            Html = "<div class=\"input-group\"><input type=\"text\" class=\"form-control\" placeholder=\"Search...\"><span class=\"input-group-btn\"><button class=\"btn btn-default\"><i class=\"fa fa-search\"></i></button></span></div>"
                        }
                    )
                );

                #endregion
            }

            _dbContext.SaveChanges();
        }
    }
}
