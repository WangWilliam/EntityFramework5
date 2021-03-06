﻿// PkgCmdID.cs
// MUST match PkgCmdID.h
namespace Microsoft.DbContextPackage
{
    internal static class PkgCmdIDList
    {
        public const uint cmdidViewEntityDataModel = 0x100;
        public const uint cmdidViewEntityDataModelXml = 0x200;
        public const uint cmdidPrecompileEntityDataModelViews = 0x300;
        public const uint cmdidViewEntityModelDdl = 0x400;
        public const uint cmdidReverseEngineerCodeFirst = 0x001;
        public const uint cmdidCustomizeReverseEngineerTemplates = 0x005;
    }
}