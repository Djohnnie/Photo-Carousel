﻿using Microsoft.Extensions.Configuration;

namespace PhotoCarousel.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        private const string DB_CONNECTION_STRING = nameof(DB_CONNECTION_STRING);
        private const string PHOTO_ROOT_PATH = nameof(PHOTO_ROOT_PATH);
        private const string THUMBNAIL_PATH = nameof(THUMBNAIL_PATH);
        private const string INDEXER_INTERVAL_IN_SECONDS = nameof(INDEXER_INTERVAL_IN_SECONDS);
        private const string THUMBNAIL_INTERVAL_IN_SECONDS = nameof(THUMBNAIL_INTERVAL_IN_SECONDS);
        private const string SCHEDULER_INTERVAL_IN_SECONDS = nameof(SCHEDULER_INTERVAL_IN_SECONDS);
        private const string CLEANUP_INTERVAL_IN_SECONDS = nameof(CLEANUP_INTERVAL_IN_SECONDS);
        private const string THUMBNAIL_SIZE = nameof(THUMBNAIL_SIZE);
        private const string PHOTO_SLIDESHOW_DURATION = nameof(PHOTO_SLIDESHOW_DURATION);

        public static string GetConnectionString(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(DB_CONNECTION_STRING);
        }

        public static string GetPhotoRootPath(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(PHOTO_ROOT_PATH);
        }

        public static string GetThumbnailPath(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(THUMBNAIL_PATH);
        }

        public static int GetIndexerIntervalInSeconds(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(INDEXER_INTERVAL_IN_SECONDS);
        }

        public static int GetThumbnailerIntervalInSeconds(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(THUMBNAIL_INTERVAL_IN_SECONDS);
        }

        public static int GetSchedulerIntervalInSeconds(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(SCHEDULER_INTERVAL_IN_SECONDS);
        }
        
        public static int GetCleanupIntervalInSeconds(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(CLEANUP_INTERVAL_IN_SECONDS);
        }

        public static int GetThumbnailSize(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(THUMBNAIL_SIZE);
        }

        public static int GetPhotoSlideshowDuration(this IConfiguration configuration)
        {
            return configuration.GetValue<int>(PHOTO_SLIDESHOW_DURATION);
        }
    }
}