/*
 * EM Hasher
 * Copyright © 2026 Enda Mullally (em.apps@outlook.ie)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "HashExplorerCommand.h"

#include <shlwapi.h>
#include <shellapi.h>
#include <pathcch.h>
#include <wininet.h> // InternetCanonicalizeUrl

using namespace Microsoft::WRL;

// Defined in dllmain.cpp; the DLL module handle used to locate the packaged icon.
extern HINSTANCE g_hInst;

namespace
{
	// Menu item title shown in the Windows 11 context menu.
	constexpr wchar_t kMenuTitle[] = L"Hash with EM Hasher";

	// Custom protocol used to launch the packaged EM Hasher app.
	constexpr wchar_t kProtocolScheme[] = L"emhasher://hash?file=";

	// Icon path relative to the package root (where the DLL is deployed).
	// The DLL deploys under <packageRoot>\EM.Hasher.ShellExtension\, while the
	// icon lives at <packageRoot>\Assets\, so step up one directory to reach it.
	constexpr wchar_t kIconRelativePath[] = L"..\\Assets\\EMHasher.ico";

	// URL-encode a raw filesystem path so it survives protocol activation.
	std::wstring UrlEncode(const std::wstring& value)
	{
		DWORD length = 1;
		// First call to determine the required buffer length.
		InternetCanonicalizeUrlW(value.c_str(), nullptr, &length, ICU_ENCODE_PERCENT | ICU_ENCODE_SPACES_ONLY);

		std::wstring encoded;
		encoded.resize(length);

		if (InternetCanonicalizeUrlW(value.c_str(), encoded.data(), &length, ICU_ENCODE_PERCENT | ICU_ENCODE_SPACES_ONLY))
		{
			encoded.resize(length);
			return encoded;
		}

		return value;
	}
}

std::wstring HashExplorerCommand::GetFirstFilePath(_In_opt_ IShellItemArray* items)
{
	std::wstring result;

	if (items == nullptr)
	{
		return result;
	}

	DWORD count = 0;
	if (FAILED(items->GetCount(&count)) || count == 0)
	{
		return result;
	}

	ComPtr<IShellItem> item;
	if (FAILED(items->GetItemAt(0, &item)))
	{
		return result;
	}

	PWSTR filePath = nullptr;
	if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &filePath)) && filePath != nullptr)
	{
		result.assign(filePath);
		CoTaskMemFree(filePath);
	}

	return result;
}

IFACEMETHODIMP HashExplorerCommand::GetTitle(_In_opt_ IShellItemArray* /*items*/, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	return SHStrDupW(kMenuTitle, name);
}

IFACEMETHODIMP HashExplorerCommand::GetIcon(_In_opt_ IShellItemArray* /*items*/, _Outptr_result_nullonfailure_ PWSTR* icon)
{
	*icon = nullptr;

	// Resolve the icon next to this DLL: <packageRoot>\Assets\EMHasher.ico.
	wchar_t modulePath[MAX_PATH] = {};
	if (GetModuleFileNameW(g_hInst, modulePath, ARRAYSIZE(modulePath)) == 0)
	{
		return E_FAIL;
	}

	// Strip the DLL file name to get the containing directory.
	if (FAILED(PathCchRemoveFileSpec(modulePath, ARRAYSIZE(modulePath))))
	{
		return E_FAIL;
	}

	wchar_t iconPath[MAX_PATH] = {};
	if (FAILED(PathCchCombine(iconPath, ARRAYSIZE(iconPath), modulePath, kIconRelativePath)))
	{
		return E_FAIL;
	}

	return SHStrDupW(iconPath, icon);
}

IFACEMETHODIMP HashExplorerCommand::GetToolTip(_In_opt_ IShellItemArray* /*items*/, _Outptr_result_nullonfailure_ PWSTR* infoTip)
{
	*infoTip = nullptr;
	return E_NOTIMPL;
}

IFACEMETHODIMP HashExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName)
{
	*guidCommandName = __uuidof(HashExplorerCommand);
	return S_OK;
}

IFACEMETHODIMP HashExplorerCommand::GetState(_In_opt_ IShellItemArray* items, _In_ BOOL /*okToBeSlow*/, _Out_ EXPCMDSTATE* cmdState)
{
	// Always visible; enabled only for a single-file selection.
	DWORD count = 0;
	if (items != nullptr && SUCCEEDED(items->GetCount(&count)) && count == 1)
	{
		*cmdState = ECS_ENABLED;
	}
	else
	{
		*cmdState = ECS_DISABLED;
	}

	return S_OK;
}

IFACEMETHODIMP HashExplorerCommand::Invoke(_In_opt_ IShellItemArray* items, _In_opt_ IBindCtx* /*bindCtx*/)
{
	const std::wstring filePath = GetFirstFilePath(items);
	if (filePath.empty())
	{
		return S_OK;
	}

	const std::wstring uri = std::wstring(kProtocolScheme) + UrlEncode(filePath);

	SHELLEXECUTEINFOW info = { sizeof(info) };
	info.fMask = SEE_MASK_NOASYNC | SEE_MASK_FLAG_NO_UI;
	info.lpVerb = L"open";
	info.lpFile = uri.c_str();
	info.nShow = SW_SHOWNORMAL;

	ShellExecuteExW(&info);

	return S_OK;
}

IFACEMETHODIMP HashExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags)
{
	*flags = ECF_DEFAULT;
	return S_OK;
}

IFACEMETHODIMP HashExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	return E_NOTIMPL;
}
