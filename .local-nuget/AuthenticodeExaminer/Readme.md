# Readme

`Forked-AuthenticodeExaminer.0.4.0.nupkg` is a fork of:

https://github.com/vcsjones/AuthenticodeExaminer

Created by Kevin Jones, licensed under the MIT License.

**Fork repository:**

https://github.com/enda-mullally/AuthenticodeExaminer

This fork includes the following important security fixes:

* `System.Security.Cryptography.Pkcs` 8.0.1 → 10.0.12
* `System.Security.Cryptography.Xml` 8.0.1 → 10.0.12

The following issues were also fixed (07/Oct/2026)

https://github.com/vcsjones/AuthenticodeExaminer/issues/23
Special thanks to: [jhennessey](https://github.com/jhennessey) for this fix.

Additionally, it includes miscellaneous package upgrades and a manual GitHub Actions workflow to build the NuGet package using Visual Studio 2018.

See the full diff:

https://github.com/vcsjones/AuthenticodeExaminer/compare/main...enda-mullally:AuthenticodeExaminer:main

# **Misc note:**

I changed the package ID from `AuthenticodeExaminer` to `Forked-AuthenticodeExaminer` to ensure that the local NuGet package is restored instead of the version published on nuget.org.


# **Verification:**

https://github.com/enda-mullally/AuthenticodeExaminer/actions/runs/37656757933

Artifact : AuthenticodeExaminer-Nuget-Package (zipped)
Digest	 : sha256:sha256:9f086584f5f5c305116b459f25bcf599d0b1f0c7875d3a6b492df34d2cb293ca

-> Forked-AuthenticodeExaminer.0.4.0.nupkg
   sha256:355f8285ba8f54b222dfb1ea20e20a0f5cf058563ffab83531434d9d4fc7bdc2
