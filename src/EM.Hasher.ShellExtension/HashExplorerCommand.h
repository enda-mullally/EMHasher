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

#pragma once

#include <windows.h>
#include <shobjidl_core.h>
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <string>

// {AB3F812D-6B1D-40E8-B040-B4CCC9ACB1D2}
// CLSID of the "Hash with EM Hasher" explorer command handler.
// This MUST match the CLSID registered in Package.appxmanifest.
class __declspec(uuid("AB3F812D-6B1D-40E8-B040-B4CCC9ACB1D2")) HashExplorerCommand final :
	public Microsoft::WRL::RuntimeClass<
		Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom>,
		IExplorerCommand>
{
public:
	// IExplorerCommand
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon) override;
	IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* infoTip) override;
	IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) override;
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* items, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* items, _In_opt_ IBindCtx* bindCtx) override;
	IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags) override;
	IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) override;

private:
	// Returns the filesystem path of the first selected item, or an empty string.
	static std::wstring GetFirstFilePath(_In_opt_ IShellItemArray* items);
};
