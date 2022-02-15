using AutoMapper;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Application.Queries;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateApplication.Command, Domain.Entities.Application>();
            CreateMap<UpdateApplication.Command, Domain.Entities.Application>();
            CreateMap<Domain.Entities.Application, GetAllApplications.Model>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ApplicationId));

            CreateMap<Domain.Entities.ProcessDefinition, GetApplicationProcessDefinitions.Model>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ProcessDefinitionId));

            CreateMap<CreateEventDefinition.Command, EventDefinition>();
            CreateMap<UpdateEventDefinition.Command, EventDefinition>();

            CreateMap<CreateProcessDefinition.Command, ProcessDefinition>();
            CreateMap<ProcessDefinition, ProcessUpdated.Model>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ProcessDefinitionId));
            CreateMap<UpdateProcessDefinition.Command, ProcessUpdated.Model>();
            CreateMap<UpdateProcessDefinition.Command, ProcessDefinition>()
                .ForMember(dest => dest.ProcessDefinitionId, mapConfig => mapConfig.MapFrom(source => source.Id));
            CreateMap<Domain.Entities.Application, GetAllProcessDefinitions.ApplicationModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ApplicationId));
            CreateMap<Domain.Entities.ProcessDefinition, GetAllProcessDefinitions.Model>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ProcessDefinitionId));
            CreateMap<Domain.Entities.EventDefinition, GetProcessDefinitionById.EventDefinitionModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.EventDefinitionId));
            CreateMap<Domain.Entities.TaskDefinition, GetProcessDefinitionById.TaskDefinitionModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.TaskDefinitionId));
            CreateMap<Domain.Entities.ProcessEventDefinition, GetProcessDefinitionById.ProcessEventDefinitionModel>();

            CreateMap<Domain.Entities.ProcessEventDefinition, GetProcessEventDefinitionsByProcessDefinitionId.ProcessEventDefinitionModel>();

            CreateMap<Domain.Entities.EventDefinition, GetProcessEventDefinitionsByProcessDefinitionId.EventDefinitionModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.EventDefinitionId));
            CreateMap<Domain.Entities.TaskDefinition, GetTaskDefinitionsByProcessId.TaskModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.TaskDefinitionId))
                .ForMember(dest => dest.AutomaticStart, mapConfig => mapConfig.MapFrom(source => source.AutomaticStart));
            CreateMap<Domain.Entities.ProcessDefinition, GetProcessDefinitionById.Model>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.ProcessDefinitionId));

            CreateMap<Domain.Entities.Application, GetProcessDefinitionById.ApplicationModel>();

            CreateMap<AddProcessEventDefinition.Command, ProcessEventDefinition>();
            CreateMap<UpdateProcessEventDefinition.Command, ProcessEventDefinition>();

            CreateMap<AddTaskDefinition.Command, TaskDefinition>();
            CreateMap<UpdateTaskDefinition.Command.TaskModel, TaskDefinition>()
                .ForMember(dest => dest.TaskDefinitionId, mapConfig => mapConfig.MapFrom(source => source.Id));
            CreateMap<UpdateTaskDefinition.Command.DeleteTaskModel, TaskDefinition>()
                .ForMember(dest => dest.TaskDefinitionId, mapConfig => mapConfig.MapFrom(source => source.TaskId));
            CreateMap<TaskDefinition, TaskUpdated.TaskDefinitionModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.TaskDefinitionId));
            CreateMap<EventDefinition, TaskUpdated.EventDefinitionModel>()
                .ForMember(dest => dest.Id, mapConfig => mapConfig.MapFrom(source => source.EventDefinitionId));

            CreateMap<PersistProcessEventDefinition.Command.ProcessEventDefinitionModel, ProcessEventDefinition>()
                .ForMember(dest => dest.EventDefinitionId, mapConfig => mapConfig.MapFrom(source => source.EventDefinition.Id))
                .ForMember(dest => dest.ProcessDefinition, mapConfig => mapConfig.Ignore())
                .ForMember(dest => dest.EventDefinition, mapConfig => mapConfig.Ignore());
            CreateMap<PersistProcessEventDefinition.Command.DeleteProcessEventDefinitionModel, ProcessEventDefinition>()
                .ForMember(dest => dest.ProcessIdentifierProps, mapConfig => mapConfig.Ignore())
                .ForMember(dest => dest.ProcessDefinition, mapConfig => mapConfig.Ignore())
                .ForMember(dest => dest.EventDefinition, mapConfig => mapConfig.Ignore());
            
            CreateMap<PersistProcessEventDefinition.Command.ProcessEventDefinitionModel, EventDefinitionUpdated.ProcessEventDefinitionModel>();
            CreateMap<PersistProcessEventDefinition.Command.EventDefinitionModel, EventDefinitionUpdated.EventDefinitionModel>();

            CreateMap<PersistProcessEventDefinition.Command.DeleteProcessEventDefinitionModel, EventDefinitionUpdated.DeleteProcessEventDefinitionModel>();

        }
    }
}