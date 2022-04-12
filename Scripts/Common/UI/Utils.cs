using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Gamla.UI;
using Gamla.UI.Carousel;
using UnityEngine;
using Random = System.Random;

namespace Gamla.Logic
{
    public static class Utils
    {

        public static string GenerateRandomStr()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        
        public static bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
        
        public static bool IsValidPassword(string password)
        {
            string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";
            if (!string.IsNullOrEmpty(password))
            {
                if (!Regex.IsMatch(password, patternPassword))
                {
                    return false;
                }
                
            }
            return true;
        }
        
        public static int GetPlainUnixTime
        {
            get
            {
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (int)timeSpan.TotalSeconds;
            }
        }
        
        public static bool IsValidPhone(string number)
        {
            return Regex.Match(number, @"^[1-9]+([0-9]+){0,1}$").Success;
        }

        public static GridCarouselPresenter CreateGridViewPresenter(
            ICarouselRootView carouselRootView,
            IGridDataSource dataSource,
            string scrollElementLoadPath,
            GridLineParams gridLineParams,
            Action<GUIView, int, int> onElementWillShown,
            Action<GUIView, int, int> onElementWillHidden = null
        )
        {
            var carouselView = carouselRootView.carouselView;

            var scrollElementFactory = SimpleGridElementFactory.CreateMonoCell(
                scrollElementLoadPath,
                gridLineParams,
                carouselView,
                dataSource,
                onElementWillShown,
                onElementWillHidden);

            var carouselPresenter = new GridCarouselPresenter(dataSource, scrollElementFactory);

            carouselRootView.carouselRootView.onShow += dialog =>
            {
                ((ICarouselRootView)dialog).carouselView.Bind(carouselPresenter);
                carouselPresenter.Bind(carouselView);
            };
            carouselRootView.carouselRootView.onClosed += dialog =>
            {
                ((ICarouselRootView)dialog).carouselView.UnBind();
                carouselPresenter.UnBind();
            };

            return carouselPresenter;
        }
        
        internal static void AddOrUpdate< TKey, TValue >( this IDictionary< TKey, TValue > source, KeyValuePair< TKey, TValue > pair )
        {
            if( source.ContainsKey( pair.Key ) )
            {
                source[ pair.Key ] = pair.Value;
            }
            else
            {
                source.Add( pair );
            }
        }

        internal static void AddOrUpdate< TKey, TValue >( this IDictionary< TKey, TValue > source, TKey key, TValue value ) => source.AddOrUpdate( new KeyValuePair< TKey, TValue >( key, value ) );

        
        public static bool CheckFileExists(string fileName)
        {
            return File.Exists(fileName);
        }
    
        public static byte[] ReadFile(string fileName)
        {
            byte[] bytes = null;

            try
            {
                bytes = File.ReadAllBytes(fileName);
            }
            catch (Exception exception)
            {
                Debug.LogError("ReadFile " + fileName + "\n" +exception);
            }

            return bytes;
        }
    
        public static void SaveFile(string fileName, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(fileName, bytes);
            }
            catch (Exception exception)
            {
                Debug.LogError("SaveFile " + fileName + "\n" +exception);
            }
        }
    
        public static void SaveFileString(string fileName, string data)
        {
            try
            {
                File.WriteAllText(fileName, data);
            }
            catch (Exception exception)
            {
                Debug.LogError("SaveFile String " + fileName + "\n" +exception);
            }
        }
    }
}