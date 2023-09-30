using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
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
		{
			return;
		}

		if (objectResult.Value is object?[] values)
		{
			for (int i = 0; i < values.Length; i++)
				values[i] = SanitizeValue(values[i]);
		}
		else
			objectResult.Value = SanitizeValue(objectResult.Value);
	}

	private object? SanitizeValue(object? value)
	{
		if (value == null)
			return value;

		if (value is string stringValue)
			return htmlEncoder.Encode(stringValue);

		IEnumerable<PropertyInfo> props;

		if (value is IEnumerable<object> collection)
		{
			foreach (var item in collection)
			{
				props = GetProperties(item);
				SanitizeProps(item, props);
			}
		}
		else
		{
			props = GetProperties(value);
			SanitizeProps(value, props);
		}

		return value;
	}

	private static IEnumerable<PropertyInfo> GetProperties(object value)
	{
		return value
			.GetType()
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(prop =>
					prop.PropertyType == typeof(string) ||
					prop.PropertyType.IsAssignableTo(typeof(IEnumerable)) ||
					prop.PropertyType.IsClass);
	}

	private void SanitizeProps(
		object obj,
		IEnumerable<PropertyInfo> objProps)
	{
		foreach (PropertyInfo prop in objProps)
		{
			object? sanitizedValue = SanitizeValue(prop.GetValue(obj));
			prop.SetValue(obj, sanitizedValue);
		}
	}
}
