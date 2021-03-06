﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Represents a check constraint in the <see cref="IConventionModel" />.
    /// </summary>
    public interface IConventionCheckConstraint : ICheckConstraint
    {
        /// <summary>
        ///     The <see cref="IConventionModel" /> in which this check constraint is defined.
        /// </summary>
        new IConventionModel Model { get; }

        /// <summary>
        ///     Returns the configuration source for this <see cref="IConventionCheckConstraint" />.
        /// </summary>
        /// <returns> The configuration source for <see cref="IConventionCheckConstraint" />. </returns>
        ConfigurationSource GetConfigurationSource();
    }
}
