using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleECommerce.Web.Dtos;

namespace SampleECommerce.Web.ModelBinders;

[UsedImplicitly]
public class UserDtoModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelMetadata.ModelType != typeof(UserIdDto))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (value == ValueProviderResult.None || value.FirstValue == null || !int.TryParse(value.FirstValue, out var userId))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var model = new UserIdDto { UserId = userId };
        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}
