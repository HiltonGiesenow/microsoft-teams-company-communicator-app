﻿// <copyright file="NotificationDeliveryStatusHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Helpers;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;

    /// <summary>
    /// This class helps callers to get, set, and reset a notification's delivery status in the table storage.
    /// e.g. succeeded, throttled, failed, unknown repcipient counts, and etc.
    /// </summary>
    public class NotificationDeliveryStatusHelper
    {
        private readonly SentNotificationDataRepository sentNotificationDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDeliveryStatusHelper"/> class.
        /// </summary>
        /// <param name="sentNotificationDataRepository">Sent notification data repository service.</param>
        public NotificationDeliveryStatusHelper(
            SentNotificationDataRepository sentNotificationDataRepository)
        {
            this.sentNotificationDataRepository = sentNotificationDataRepository;
        }

        /// <summary>
        /// This method gets a notification's delivery status.
        /// e.g. succeeded, throttled, failed, unknown repcipient counts, and etc.
        /// </summary>
        /// <param name="notificationId">Notification id.</param>
        /// <returns>It returns the notification delivery status DTO object.</returns>
        public async Task<NotificationDeliveryStatusDTO> GetNotificationDeliveryStatusAsync(string notificationId)
        {
            NotificationDeliveryStatusDTO notificationDeliveryStatusDTO = null;

            await this.sentNotificationDataRepository.GetAllAsync(
                notificationId,
                null,
                sentNotificationDataEntities =>
                {
                    if (sentNotificationDataEntities == null)
                    {
                        return;
                    }

                    if (notificationDeliveryStatusDTO == null)
                    {
                        notificationDeliveryStatusDTO = new NotificationDeliveryStatusDTO();
                    }

                    notificationDeliveryStatusDTO.Succeeded += sentNotificationDataEntities.Count(p => p.StatusCode.IsSucceeded());
                    notificationDeliveryStatusDTO.Throttled += sentNotificationDataEntities.Count(p => p.StatusCode.IsThrottled());
                    notificationDeliveryStatusDTO.Failed += sentNotificationDataEntities.Count(p => p.StatusCode.IsFailed());
                    notificationDeliveryStatusDTO.Unknown += sentNotificationDataEntities.Count(p => p.StatusCode.IsUnknown());
                    notificationDeliveryStatusDTO.LastSentDate = sentNotificationDataEntities.Max(p => p.SentDate != null ? p.SentDate : DateTime.MinValue);
                });

            return notificationDeliveryStatusDTO;
        }
}
}