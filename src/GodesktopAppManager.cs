using Godot;
using System;
using System.Collections.Generic;

namespace Godesktop;

public partial class GodesktopAppManager : Node
{
	private readonly Dictionary<string, GodesktopApp> activeApps = [];
	private readonly Dictionary<string, GodesktopApp> appRegistry = [];

	public Action<string> AppRegistered;
	public Action<string> AppUnregistered;
	public Action<string> AppOpened;
	public Action<string> AppClosed;

	public async override void _Ready()
	{
        GodesktopApp godesktopSettings = new()
        {
            registryName = "globalSettings",
            displayName = "globalSettings"
        };

		RegisterApp(godesktopSettings);
	}
	
	public bool RegisterApp(GodesktopApp app)
	{
		if(app.Equals(null))
		{
			GD.PrintErr($"GodesktopAppManager.cs - Cannot register a null application.");
			return false;
		}

		if(appRegistry.ContainsKey(app.registryName))
		{
			GD.PrintErr($"GodesktopAppManager.cs - Could not register application \"{app.registryName}\". App is already registered.");
			return false;
		}

		appRegistry.Add(app.registryName, app);
		AppRegistered?.Invoke(app.registryName);
		GD.Print($"Registration Successful - {app.registryName}");
		return true;
	}

	public bool UnregisterApp(GodesktopApp app)
	{
		if(app.Equals(null))
		{
			GD.PrintErr($"GodesktopAppManager.cs - Cannot remove a null application from register.");
			return false;
		}

		if(!appRegistry.ContainsKey(app.registryName))
		{
			GD.PrintErr($"GodesktopAppManager.cs - Could not remove application \"{app.registryName}\" from registry. App does not exist.");
			return false;
		}

		if(activeApps.ContainsKey(app.registryName))
		{
			GD.PrintErr($"GodesktopAppManager.cs - Could not remove application \"{app.registryName}\" from registry. App is still open! Close app instance before removing from registry.");
			return false;
		}

		appRegistry.Remove(app.registryName);
		AppUnregistered?.Invoke(app.registryName);
		GD.Print($"Unregistration Successful - {app.registryName}");
		return true;
	}

	public bool OpenApp(string registryName)
	{	
		if(!appRegistry.TryGetValue(registryName, out GodesktopApp app))
		{
			GD.PrintErr($"GodesktopAppManager.cs - App \"{registryName}\" does not exist in registry.");
			return false;
		}

		if(activeApps.ContainsKey(registryName))
		{
			GD.PrintErr($"GodesktopAppManager.cs - App is already open. Cannot open two instances of the same app.");
			return false;
		}

		//Start Open App Logic Here
		activeApps.Add(app.registryName, app);
		AppOpened?.Invoke(app.registryName);
		GD.Print($"Open App Successful - {registryName}");
		return true;
	}

	public bool CloseApp(string registryName)
	{	
		if(!appRegistry.TryGetValue(registryName, out GodesktopApp app))
		{
			GD.PrintErr($"GodesktopAppManager.cs - App \"{registryName}\" does not exist in registry.");
			return false;
		}

		if(!activeApps.ContainsKey(registryName))
		{
			GD.PrintErr($"GodesktopAppManager.cs - App \"{registryName}\" isn't currently running.");
			return false;
		}

		//Start Close App Logic Here
		activeApps.Remove(app.registryName);
		AppClosed?.Invoke(app.registryName);
		GD.Print($"Close App Successful - {registryName}");
		return true;
	}
}
