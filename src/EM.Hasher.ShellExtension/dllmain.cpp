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

using namespace Microsoft::WRL;

// Module handle for this DLL, used to resolve the packaged icon path at runtime.
HINSTANCE g_hInst = nullptr;

// Register the command handler as a classic COM class so the shell can
// CoCreate it in-process via the CLSID declared in the package manifest.
CoCreatableClass(HashExplorerCommand);

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD reason, LPVOID /*reserved*/)
{
	if (reason == DLL_PROCESS_ATTACH)
	{
		g_hInst = hInstance;
		DisableThreadLibraryCalls(hInstance);
	}

	return TRUE;
}

STDAPI DllGetClassObject(_In_ REFCLSID clsid, _In_ REFIID riid, _COM_Outptr_ void** instance)
{
	return Module<InProc>::GetModule().GetClassObject(clsid, riid, instance);
}

STDAPI DllCanUnloadNow()
{
	return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}
