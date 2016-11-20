/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using $safeprojectname$.Entities;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$.Services
{
    /// <summary>
    /// This service interface defines methods related to security operations.
    /// </summary>
    ///
    /// <threadsafety>
    /// Implementations of this interface should be effectively thread safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public interface ISecurityService
    {
        /// <summary>
        /// Authenticates user and returns populated User instance in case of success.
        /// </summary>
        ///
        /// <param name="username">The username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>The authenticated User entity, or null if cannot authenticate user.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="username"/> or <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If error occurs while accessing the persistence.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        User Authenticate(string username, string password);

        /// <summary>
        /// Checks whether non-admin user is authorized to perform specified action.
        /// </summary>
        ///
        /// <param name="action">The action to perform.</param>
        /// <returns>True if user is authorized to perform specified action; otherwise False.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="action"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="action"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        bool IsAuthorized(string action);

        /// <summary>
        /// Generates token for the given user.
        /// </summary>
        ///
        /// <param name="user">The user.</param>
        /// <returns>The generated token.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="user"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        Token GenerateToken(User user);

        /// <summary>
        /// Authenticates user and returns populated User instance in case of success.
        /// </summary>
        ///
        /// <param name="token">The token.</param>
        /// <returns>The authenticated User entity, or null if cannot authenticate user.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="token"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="token"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        User Authenticate(string token);

        /// <summary>
        /// Revokes the token.
        /// </summary>
        ///
        /// <param name="token">The token to revoke.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="token"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="token"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        void RevokeToken(string token);
    }
}
