namespace Dyvenix.Core.Api.Authorization;

/// <summary>
/// Manages permission hierarchies registered by each service module.
/// </summary>
public sealed class PermissionRegistry
{
	private readonly Dictionary<string, string[]> _implications = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Registers permission implications. Call during service startup.
	/// </summary>
	/// <param name="implications">Dictionary mapping permissions to the permissions they imply.</param>
	public void Register(IReadOnlyDictionary<string, string[]> implications)
	{
		foreach (var (permission, implied) in implications)
			_implications[permission] = implied;
	}

	/// <summary>
	/// Expands a set of permissions to include all implied permissions.
	/// </summary>
	public HashSet<string> ExpandPermissions(IEnumerable<string> permissions)
	{
		var expanded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var queue = new Queue<string>(permissions);

		while (queue.Count > 0)
		{
			var permission = queue.Dequeue();

			if (!expanded.Add(permission))
				continue;

			if (_implications.TryGetValue(permission, out var implied))
			{
				foreach (var impliedPermission in implied)
					queue.Enqueue(impliedPermission);
			}
		}

		return expanded;
	}
}
