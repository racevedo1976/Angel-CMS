using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.Helpers
{
    [HtmlTargetElement("options")]
    public class OptionsTagHelper: TagHelper
    {
        public CalendarWidgetSetting CalendarSettingsModel { get; set; }

        public OptionsTagHelper()
        {

        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "script";    // Replaces <email> with <a> tag
            output.TagMode = TagMode.StartTagAndEndTag;


            output.Content.SetHtmlContent(@"

            var options = {
 " + $@"        events_source: '/api/content/calendar/events?widgetId={CalendarSettingsModel.Id}', //  'assets/calendar/events.json',  ////  api/calendar/events
                view: '{CalendarSettingsModel.DefaultView}',
 		        tmpl_path: 'assets/calendar/tmpls/',
		        tmpl_cache: false,
                modal: '#events-modal',
                modal_type: 'template',
		        day: 'now', // '2013-03-12',
" + @"
                modal_title : function (e) { return e.title },
		        onAfterEventsLoad: function(events) {

                },

                onAfterViewLoad: function(view) {
	        	    $('#calendarTitle').text(this.getTitle());
	        		$('.btn-group button').removeClass('active');
                    $('.calendarView li').removeClass('active');

	        		$('button[data-calendar-view=""' + view + '""]').addClass('active');
                    $('li[data-calendar-view=""' + view + '""]').addClass('active');

                },
                classes: {
                    months: {
                        general: 'label'
                    }
                }


" + @"     }


            function addZero(i) {
	            if (i < 10) {
	                i = '0' + i;
                }
	            return i;
	        }
  ");









        }
    }
}
