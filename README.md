[![Build and test](https://github.com/enda-mullally/EMHasher/actions/workflows/build.yml/badge.svg)](https://github.com/enda-mullally/EMHasher/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/enda-mullally/EMHasher)](/LICENSE)
![License](https://img.shields.io/github/v/release/enda-mullally/EMHasher)
#
<p align="center">
  <img width="80" align="center" src="docs/images/AppLogo80x80.png">
</p>
<h1 align="center">
  EM Hasher
</h1>
<p align="center">
  Quickly calculate BLAKE3/CRC-32/MD5/SHA-1/SHA-256/SHA-512/SHA3-256/SHA3-512 checksums in Windows 11.
</p>

<br />

<p align="center">
  <a href="https://apps.microsoft.com/detail/9NZZHH7X25CG?referrer=appbadge&launch=true&cid=github&mode=mini">
	  <img src="https://get.microsoft.com/images/en-us%20dark.svg" width="300"/>
  </a>
</p>

## Overview

EM Hasher is a modern, simple hash calculating app (WinUI 3) built from the ground up for Windows 11. Native and fast (AOT) x64/ARM64. Quickly and easily calculate file hashes directly in Windows Explorer or on your Desktop. Choose your preferred hashing algorithms, including BLAKE3/CRC-32/MD5/SHA-1/SHA-256/SHA-512. What would you like to see next?

<br />

#### Screenshots

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_01_Main_Screen_Drop.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_02_Main_Screen.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_03_Main_Screen_Results.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_04_Main_Screen_Results_Compact.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_05_Main_Screen_Signed_Results.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_06_Main_Screen_Hash_Validation.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_07_Main_Screen_Settings_Dark.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_08_Main_Screen_Settings_HashAlgorithms_Dark.png">
</p>

<p align="center">
  <img align="center" style="border-radius: 8px; width: 100%; height: 100%;" src="docs/images/Store_09_Main_Screen_Settings_Light.png">
</p>
<br />

## Release history

### 24/Aug/2026 ###
v1.4 Build(82)
  - Added SHA3-256 and SHA3-512 hashing.
  - Other UI improvements.

Note: SHA3-256 and SHA3-512 both require Windows 11, 24H2 or higher and will be disabled on earlier versions of Windows 11.
See: https://learn.microsoft.com/en-us/windows/whats-new/whats-new-windows-11-version-24h2#sha-3-support

### 14/Aug/2026 ###
v1.3 Build(80)
  - Added hash calculation percentage (%) and proper progress bar for large files.
  - Fixed UI issue with authenticode verification for large signed files (typically > 1 GB).
  - Other UI improvements.

### 10/Aug/2026 ###
v1.2 Build(77)
  - Automatic hash validation for 'MD5' (via .md5 files) & 'SHA-256' (via .sha256 files).
  - New version label format v1.2.{build} etc
  - Misc improvements & UI fixes.

### 02/Aug/2026 ###
v1.1.75
  - Added 'BLAKE3' hashing.
  - Misc improvements & fixes.

### 15/Jul/2026 ###
v1.1.74
  - Authenticode refactor - Display Authenticode Signing Time (signed files).
  - File properties - Display file version info (File & Product version properties).
  - [UI] Improved scrolling (Hash Values).

### 14/Jun/2026 ###
v1.1.73
  - WinAppSdk v2.2.0 upgrade.
  - Package version info fixes.

### 04/Jun/2026
v1.1.72
  - Added 'SHA-1' hashing.
  - Misc bug fixes.

### 18/Apr/2026
v1.1.71
  - [UI] Renamed nav bar tab pages 'Select File' and 'Hash Values' and added better icons.
  - [UI] Settings -> Hash Algorithms -> Breadcrumb style navigation (enabling new Algorithms).
  - [UI] Reinstated compact mode.

### 25/Mar/2026
v1.1.70
  - Mini release. Settings -> About. Improved copyright notice.

### 18/Mar/2026
v1.1.69
  - Misc bug fixes - Added Digital Signature info panel for signed files.

### 02/Jan/2026
v1.1.68
  - [UI] Improved look of Settings page.
  - Fixed rendering issue when app is resized on snap. App package size fixed.

### 06/Dec/2025
v1.1.67
  - [UI] Small tooltip fix.

### 03/Dec/2025
v1.1.66
  - [UI] Improved file size information - now displaying full byte size of the selected file.
  - [UI] About. Updated and added third party license links (notices).

### 24/Nov/2025
v1.1.65
  - Added a 'Search this hash on VirusTotal' button to the SHA-256 calculation tab. Note: Only the hash value itself is searched.

### 15/Nov/2025
v1.1.64
  - Upgraded packages + .NET 10 upgrade & Added 'Show file location' button to the File Information tab.

### 06/Oct/2025
v1.1.63
  - Upgraded to WinAppSDK 1.8.1 and updated UI.

### 15/Sep/2025
v1.1.62
  - Upgraded to WinAppSDK 1.8 and added improved CopyButton control (copy hash value).

### 08/Sep/2025
v1.1.59
  - Added third party license notices -> About section.

### 04/Aug/2025
v1.1.58
  - Now free & unrestricted. Enjoy :-)

### 22/Jul/2025
v1.1.57
  - UI improvements.

### 10/Jul/2025
v1.1.56
  - Added 'Copy hash to clipboard' button.

### 23/Jun/2025 ###
v1.1.55
  - Unlimited trial (with some features restricted).

### 09/Jun/2025
v1.1.54
  - Added 'CRC-32' hashing.

### 05/Jun/2025
v1.1.53
  - Improvements and bug fixes!

### 16/May/2025
v1.1.49
  - Added 'SHA-512' hashing.

### 13/May/2025
v1.1.48
  - Performance release! Managed to squeeze out a 50% perf improvement in some cases.

### 08/May/2025
v1.1.47
  - First public release. Native (AOT) compiled app, targeting x64 and ARM64.
