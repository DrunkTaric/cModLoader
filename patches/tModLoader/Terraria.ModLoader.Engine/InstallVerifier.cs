﻿using ReLogic.OS;
using Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria.ModLoader.Engine
{
	internal static class InstallVerifier
	{
		const string ContentDirectory = "Content";

		private static bool? isValid;
		public static bool IsValid => isValid ?? (isValid = InstallCheck()).Value;
		public static bool IsGoG = false;
		public static bool IsSteam = false;

		private static string steamAPIPath;
		private static byte[] steamAPIHash;
		private static byte[] gogHash;
		private static byte[] steamHash;

		static InstallVerifier()
		{
			if (Platform.IsWindows) {
				steamAPIPath = "steam_api.dll";
				steamAPIHash = ToByteArray("7B857C897BC69313E4936DC3DCCE5193");
				gogHash = ToByteArray("0d4005c39a12a334d9bfd42dd5b656cc"); // Don't forget to update CheckExe in CheckGoG
				steamHash = ToByteArray("5f321196521790a18a19d44fd066044e");
			}
			else if (Platform.IsOSX) {
				steamAPIPath = "osx/libsteam_api.dylib";
				steamAPIHash = ToByteArray("4EECD26A0CDF89F90D4FF26ECAD37BE0");
				gogHash = ToByteArray("f483f6f795e5c045b73c330015e852a6");
				steamHash = ToByteArray("c3b967ddc50f400dc1575de05979ee47");
			}
			else if (Platform.IsLinux) {
				steamAPIPath = "lib/libsteam_api.so";
				steamAPIHash = ToByteArray("7B74FD4C207D22DB91B4B649A44467F6");
				gogHash = ToByteArray("56794421993db33b7607d1be233b586d");
				steamHash = ToByteArray("b08ed3b4fe5417e7cd56e06ad99f2ab7");
			}
			else {
				string message = Language.GetTextValue("tModLoader.UnknownVerificationOS");
				Logging.tML.Fatal(message);
				Exit(message, string.Empty);
			}
		}

		private static bool HashMatchesFile(string path, byte[] hash)
		{
			using (var md5 = MD5.Create())
			using (var stream = File.OpenRead(path))
				return hash.SequenceEqual(md5.ComputeHash(stream));
		}

		private static byte[] ToByteArray(string hexString)
		{
			byte[] retval = new byte[hexString.Length / 2];
			for (int i = 0; i < hexString.Length; i += 2)
				retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
			return retval;
		}
		private static void Exit(string errorMessage, string extraMessage)
		{
			errorMessage += $"\r\n\r\n{extraMessage}";
			Logging.tML.Fatal(errorMessage);
			UI.Interface.MessageBoxShow(errorMessage);
			Environment.Exit(1);
		}

		private static bool InstallCheck()
		{
			// Check if the content directory is present which is required
			if (!Directory.Exists(ContentDirectory)) {
				Exit(Language.GetTextValue("tModLoader.ContentFolderNotFoundInstallCheck", ContentDirectory), Language.GetTextValue("tModLoader.DefaultExtraMessage"));
				return false;
			}
			return CheckGoG();
		}

		// Check if GOG install or manual install is correct
		private static bool CheckGoG()
		{
			Logging.tML.Info("Checking GOG or manual installation...");
			IsGoG = true;

			const string DefaultExe = "Terraria.exe";
			string CheckExe = $"Terraria_1.4.3.6_B.exe"; // This should match the hashes. {Main.versionNumber}
			string vanillaPath = File.Exists(CheckExe) ? CheckExe : DefaultExe;

			// If .exe not present, check Terraria directory (Side-by-Side Manual Install)
			if (!File.Exists(vanillaPath)) {
				vanillaPath = Path.Combine("..", "Terraria");
#if MAC
				// GOG installs to /Applications/Terraria.app, Steam installs to /Applications/Terraria/Terraria.app
				// working directory is /Applications/tModLoader/tModLoader.app/Contents/MacOS/ for steam manual installs
				// working directory is /Applications/tModLoader.app/Contents/MacOS/ for GOG installs
				// Vanilla .exe files are in /Contents/Resources/, not /Contents/MacOS/
				if (Directory.Exists("../../../../Terraria/Terraria.app/")) {
					vanillaPath = "../../../../Terraria/Terraria.app/Contents/Resources/";
					Logging.tML.Info($"Mac installation location found at {vanillaPath}, assuming Steam manual install");
				}
				else if (Directory.Exists("../../../Terraria.app/")) {
					vanillaPath = "../../../Terraria.app/Contents/Resources/";
					Logging.tML.Info($"Mac installation location found at {vanillaPath}, assuming GOG manual install");
				}
				else {
					Logging.tML.Info($"Mac installation location not found.");
				}
#endif
				string defaultExe = Path.Combine(vanillaPath, DefaultExe);
				string checkExe = Path.Combine(vanillaPath, CheckExe);
				vanillaPath = File.Exists(checkExe) ? checkExe : defaultExe;
			}
			// If .exe not present check parent directory (Nested Manual Install)
			if (!File.Exists(vanillaPath)) {
				string defaultExe = Path.Combine("..", DefaultExe);
				string checkExe = Path.Combine("..", CheckExe);
				vanillaPath = File.Exists(checkExe) ? checkExe : defaultExe;
			}

			if (Path.GetFileName(vanillaPath) != CheckExe) {
				string pathToCheckExe = Path.Combine(Path.GetDirectoryName(vanillaPath), CheckExe);
				Logging.tML.Info($"Backing up {Path.GetFileName(vanillaPath)} to {CheckExe}");
				File.Copy(vanillaPath, pathToCheckExe);
			}

			Logging.tML.Info("Cracked Version is looking fine ;).");
			return true;
		}
	}
}
