using App.Contracts;
using App.Exceptions;
using DataAccess;
using Domain;

namespace App;

public class RequestValidator
{
    private readonly ApplicationRepository _applicationRepository;

    public RequestValidator(ApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public void ValidateCreateApplicationRequest(CreateApplicationRequestDto request)
    {
        if (request.Author is null || request.Author == Guid.Empty)
        {
            throw new BadRequestException("Не заполнен идентификатор пользователя!");
        }

        if (_applicationRepository.CheckAuthorsApplications(request.Author.Value))
        {
            throw new BadRequestException("У этого автора уже есть черновики!");
        }
        
        if (string.IsNullOrWhiteSpace(request.Activity) 
           && string.IsNullOrWhiteSpace(request.Name)
           && string.IsNullOrWhiteSpace(request.Description)
           && string.IsNullOrWhiteSpace(request.Outline))
        {
            throw new BadRequestException("Необходимо заполнить еще одно поле!");
        }
    }

    public static void ValidateEditApplicationRequest(Applications? editableApp)
    {
        ValidateApplicationExistence(editableApp);

        if (editableApp.Status == ApplicationStatus.OnSubmitting)
        {
            throw new BadRequestException("Запрещено редактирование или удаление отправленной на рассмотрение заявки!");
        }
    }

    public static void ValidateEditedApplication(Applications editedApp)
    {
        if (editedApp.Activity is null 
            && string.IsNullOrWhiteSpace(editedApp.Name)
            && string.IsNullOrWhiteSpace(editedApp.Description)
            && string.IsNullOrWhiteSpace(editedApp.Outline))
        {
            throw new BadRequestException("Необходимо заполнить еще одно поле!");
        }
    }

    public static void ValidateSubmitRequest(Applications? appForSubmit)
    {
        ValidateApplicationExistence(appForSubmit);
            
        if (appForSubmit.Activity is null
            || string.IsNullOrWhiteSpace(appForSubmit.Name)
            || string.IsNullOrWhiteSpace(appForSubmit.Outline))
        {
            throw new BadRequestException("Для отправки на рассмотрение заполните обязательные для заполнения поля!");
        }
    }

    public static void ValidateFilterRequest(DateTime? submittedAfter,
        DateTime? unsubmittedOlder)
    {
        if (submittedAfter is not null && unsubmittedOlder is not null)
        {
            throw new BadRequestException("Необходимо заполнить только один фильтр!");
        }

        if (submittedAfter is null && unsubmittedOlder is null)
        {
            throw new BadRequestException("Необходимо заполнить хотя бы один фильтр!");
        }
    }

    public static void ValidateApplicationExistence(Applications? application)
    {
        if (application is null)
        {
            throw new NotFoundApplicationException("Заявка не найдена");
        }
    }
}