using System.ComponentModel;
using System.Reflection;

namespace Permission.Domain.Enums;

public enum ActionType
{
	[Description(nameof(View))]
	View,
	[Description(nameof(Manage))]
	Manage,
	[Description(nameof(Assign))]
	Assign
}

public static class EnumExtensions
{
	public static string GetDescription(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attr = field?.GetCustomAttribute<DescriptionAttribute>();
		return attr?.Description ?? value.ToString();
	}
}

