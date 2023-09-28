using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text.Encodings.Web;

namespace HotelBooking.WebApi.Filters;

public class HtmlSanitizeResultFilter : IResultFilter
{
	private readonly HtmlEncoder htmlEncoder;

	public HtmlSanitizeResultFilter(HtmlEncoder htmlEncoder)
		=> this.htmlEncoder = htmlEncoder;

	public void OnResultExecuted(ResultExecutedContext context)
	{ }

	public void OnResultExecuting(ResultExecutingContext context)
	{
		if (context.Result is not ObjectResult objectResult)
			return;

		if (objectResult.StatusCode != StatusCodes.Status200OK &&
			objectResult.StatusCode != StatusCodes.Status201Created)
			return;

		objectResult.Value = SanitizeValue(objectResult.Value);
	}

	private object? SanitizeValue(object? value)
	{
		if (value == null)
			return value;

		if (value is string stringValue)
			return htmlEncoder.Encode(stringValue);

		if (value is object?[] values)
		{
			for (int i = 0; i < values.Length; i++)
				values[i] = SanitizeValue(values[i]);

			return values;
		}

		Type type = value.GetType();

		if (type.IsClass)
		{
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				object? sanitizedValue = SanitizeValue(property.GetValue(value));
				property.SetValue(value, sanitizedValue);
			}
		}

		return value;
	}
}
