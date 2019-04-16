using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Assignments.Data
{
    public static class AssignmentsAutomapper
    {
        // TO DO: Calling Mapper.Initialize will reset existing mappings.  We need to create a builder object to allow
        // us to build the mappings and then initialize the mapper in one place of the code.
        public static void ConfigureAutomapper()
        {
            Mapper.Initialize(config =>
            {
                //config.CreateMissingTypeMaps = true;

                config.CreateMap<Assignment, AssignmentListItemViewModel>()
                    .ForMember(dest => dest.CreatedDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.CreatedUTC, src.TimeZoneId, DateTime.Now)))
                    .ForMember(dest => dest.DueDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.DueUTC, src.TimeZoneId, DateTime.Now)));

                config.CreateMap<AssignmentListItemViewModel, Assignment>()
                    .ForMember(dest => dest.CreatedUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.CreatedDT, src.TimeZoneId, DateTime.UtcNow)))
                    .ForMember(dest => dest.DueUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.DueDT, src.TimeZoneId, DateTime.UtcNow)));

                config.CreateMap<Assignment, AssignmentDetailsViewModel>()
                   .ForMember(dest => dest.DueDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.DueUTC, src.TimeZoneId, DateTime.Now)));

                config.CreateMap<AssignmentDetailsViewModel, Assignment>()
                   .ForMember(dest => dest.DueUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.DueDT, src.TimeZoneId, DateTime.UtcNow)));
            });
        }

        public static AssignmentListItemViewModel CloneToAssignmentListItemViewModel(this Assignment source)
        {
            var model = new AssignmentListItemViewModel();
            model.Id = source.Id;
            model.Title = source.Title;
            model.Status = source.Status;
            model.TimeZoneId = source.TimeZoneId;
            model.CreatedDT = TimeZoneHelper.ConvertFromUTC(source.CreatedUTC, source.TimeZoneId);
            model.DueDT = TimeZoneHelper.ConvertFromUTC(source.DueUTC, source.TimeZoneId);
            return model;
        }

        public static ICollection<AssignmentListItemViewModel> CloneToAssignmentListItemViewModel(this IEnumerable<Assignment> source)
        {
            var model = new List<AssignmentListItemViewModel>();
            foreach (var item in source)
                model.Add(item.CloneToAssignmentListItemViewModel());
            return model;
        }

        public static Assignment CloneToAssignemnt(this AssignmentListItemViewModel source)
        {
            var model = new Assignment();
            model.Id = source.Id;
            model.Title = source.Title;
            model.Status = source.Status;
            model.TimeZoneId = source.TimeZoneId;
            model.CreatedUTC = TimeZoneHelper.ConvertToUTC(source.CreatedDT, source.TimeZoneId);
            model.DueUTC = TimeZoneHelper.ConvertToUTC(source.DueDT, source.TimeZoneId);
            return model;
        }

        public static AssignmentDetailsViewModel CloneToAssignmentDetailsViewModel(this Assignment source)
        {
            var model = new AssignmentDetailsViewModel();
            model.Id = source.Id;
            model.OwnerId = source.OwnerId;
            model.OwnerLevel = source.OwnerLevel;
            model.CreatedBy = source.CreatedBy;
            model.CreatedUTC = source.CreatedUTC;
            model.Title = source.Title;
            model.AssignmentBody = source.AssignmentBody;
            model.Status = source.Status;
            model.AllowComments = source.AllowComments;
            model.NotificationId = source.NotificationId;
            model.SendNotification = source.SendNotification;
            model.TimeZoneId = source.TimeZoneId;
            model.DueDT = TimeZoneHelper.ConvertFromUTC(source.DueUTC, source.TimeZoneId);
            return model;
        }

        public static Assignment CloneToAssignment(this AssignmentDetailsViewModel source)
        {
            var model = new Assignment();
            model.Id = source.Id;
            model.OwnerId = source.OwnerId;
            model.OwnerLevel = source.OwnerLevel;
            model.CreatedBy = source.CreatedBy;
            model.CreatedUTC = source.CreatedUTC;
            model.Title = source.Title;
            model.AssignmentBody = source.AssignmentBody;
            model.Status = source.Status;
            model.AllowComments = source.AllowComments;
            model.NotificationId = source.NotificationId;
            model.SendNotification = source.SendNotification;
            model.TimeZoneId = source.TimeZoneId;
            model.DueUTC = TimeZoneHelper.ConvertToUTC(source.DueDT, source.TimeZoneId);
            return model;
        }

    }
}


