# GMJS Object Controller System - Data Preparation Tool

This tool helps you

  - create the Site Data needed to load onto Object Controller boards.
  - program Object Controller boards with their Unit ID, server IP and MAC address or SSID/password
  - optionally preload the Site Data.

To build a release

    msbuild IPOCS_Programmer/IPOCS_Programmer.csproj /property:PublishDir="publish/" /target:clean;publish