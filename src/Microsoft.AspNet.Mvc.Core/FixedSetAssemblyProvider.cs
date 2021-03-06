// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// A <see cref="IAssemblyProvider"/> with a fixed set of candidate assemblies. 
    /// </summary>
    public class FixedSetAssemblyProvider : IAssemblyProvider
    {
        /// <summary>
        /// Gets the list of candidate assemblies.
        /// </summary>
        public IList<Assembly> CandidateAssemblies { get; } = new List<Assembly>();

        IEnumerable<Assembly> IAssemblyProvider.CandidateAssemblies => CandidateAssemblies;
    }
}
