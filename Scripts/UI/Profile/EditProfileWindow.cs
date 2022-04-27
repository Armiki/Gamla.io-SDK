using System;
using System.Collections;
using System.IO;
using Gamla.GUI.ImageCropper;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class EditProfileWindow : GUIView
    {
        public event Action<UserInfo> onSaveClick;
        public event Action<UserInfo> onCancelSaveClick;
        public event Action onAddressChangeClick;
        public event Action onPasswordChangeClick;
        public event Action onLogOutClick;
        public event Action onRequestNewData;

        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private ValidateInputWidget _publicUserName;
        [SerializeField] private Image _flag;
        [SerializeField] private Button _changeAvatarBtn;
        [SerializeField] private AvatarComponent _avatar;
        [SerializeField] private ValidateInputWidget _firstNameInput;
        [SerializeField] private ValidateInputWidget _lastNameInput;
        [SerializeField] private ValidateInputWidget _emailInput;
        [SerializeField] private Button _changePasswordBtn;
        [SerializeField] private ValidateInputWidget _phoneInput;
        [SerializeField] private Text _adressText;
        [SerializeField] private GameObject _emptyAdressText;
        [SerializeField] private Button _changeAdressBtn;
        [SerializeField] private Button _agreeBtn;
        [SerializeField] private GameObject _agreeCheck;
        [SerializeField] private Button _logOut;
        [SerializeField] private Button _saveBtn;
        
        [SerializeField] private RecolorItem _agreeText;

        private UserInfo _userData;
        private string _changedName;
        private string _changedFirstName;
        private string _changedLastName;
        private string _changedPhone;
        private string _changedEmail;
        private bool _isAgree;
        private string _changedAddress;
        
        public void Start()
        {
            var flag = GUIConstants.guiSettings.GetFlagByCurrentCulture();
            if (flag != null)
            {
                _flag.sprite = flag;
            }
            _saveBtn.gameObject.SetActive(false);
            _isAgree = true;
            _agreeCheck.SetActive(_isAgree);
            
            _saveBtn.onClick.RemoveAllListeners();
            _saveBtn.onClick.AddListener(() =>
            {
                if (_isAgree)
                {
                    EventManager.OnProfileUpdate.Del(OnProfileUpdated);
                    EventManager.OnProfileUpdate.Subscribe(OnProfileUpdated);
                    onSaveClick?.Invoke(new UserInfo
                    {
                        name = _changedName,
                        innerUserInfo = new InnerUserInfo
                        {
                            firstName = _changedFirstName,
                            lastName = _changedLastName,
                            email = _changedEmail,
                            phone = _changedPhone,
                            address = _changedAddress
                        }
                    });
                }
                else
                {
                    //_agreeText.recolorFilter = _isAgree ? "textColorPrimary" : "textColorError";
                    _agreeText.recolorFilter = "textColorError";
                    _agreeText.Recolor();
                }
            });
            
            _changePasswordBtn.onClick.RemoveAllListeners();
            _changePasswordBtn.onClick.AddListener(() => onPasswordChangeClick?.Invoke());

            _changeAdressBtn.onClick.RemoveAllListeners();
            _changeAdressBtn.onClick.AddListener(() => onAddressChangeClick?.Invoke());
            
            _agreeBtn.onClick.RemoveAllListeners();
            _agreeBtn.onClick.AddListener(() =>
            {
                _isAgree = !_isAgree;
                _agreeCheck.SetActive(_isAgree);
                _agreeText.recolorFilter = "textColorPrimary";
                _agreeText.Recolor();
            });

            _logOut.onClick.RemoveAllListeners();
            _logOut.onClick.AddListener(() =>
            {
                _isAgree = false;
                onLogOutClick?.Invoke();
                Close();
            });

            _publicUserName.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _publicUserName.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            _firstNameInput.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _firstNameInput.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            _lastNameInput.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _lastNameInput.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            _emailInput.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _emailInput.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            _phoneInput.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _phoneInput.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            _publicUserName.onInputChange += s =>
            {
                if (_publicUserName.Validate())
                {
                    _changedName = s;
                    CheckChanges();
                }
            };

            _firstNameInput.onInputChange += s =>
            {
                if (_firstNameInput.Validate())
                {
                    _changedFirstName = s;
                    CheckChanges();
                }
            };
            
            _lastNameInput.onInputChange += s =>
            {
                if (_lastNameInput.Validate())
                {
                    _changedLastName = s;
                    CheckChanges();
                }
            };
            
            _emailInput.onInputChange += s =>
            {
                if (_emailInput.Validate())
                {
                    _changedEmail = s;
                    CheckChanges();
                }
            };
            
            _phoneInput.onInputChange += s =>
            {
                if (_phoneInput.Validate())
                {
                   _changedPhone = s;
                    CheckChanges();
                }
            };
            
            _changeAvatarBtn.onClick.AddListener(() =>
                {
                    if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read) ==
                        NativeGallery.Permission.Granted)
                    {
                        NativeGallery.GetImageFromGallery(callback =>
                        {
                            if (callback == null) return;
                            Debug.LogWarning(callback);
                            var image = NativeGallery.LoadImageAtPath(callback, -1, false);
                            CroppImage(image);
                        });
                    }
                });

            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read) == NativeGallery.Permission.ShouldAsk)
            {
                NativeGallery.RequestPermission(NativeGallery.PermissionType.Read);
            }
        }

        void CroppImage(Texture2D image)
        {
            ImageCropper.Instance.Show(image, (bool result, Texture originalImage, Texture2D croppedImage) =>
                {
                    if (result) {
                        _avatar.avatar.texture = croppedImage;
                        byte[] bytes = croppedImage.EncodeToPNG();
                        string enc = Convert.ToBase64String(bytes);
                        ServerCommand.ResetAvatar(enc);
                    }
                },
                settings: new ImageCropper.Settings()
                {
                    ovalSelection = true,
                    autoZoomEnabled = true,
                    markTextureNonReadable = false,
                    imageBackground = Color.clear,
                    selectionMinAspectRatio = 1,
                    selectionMaxAspectRatio = 1,
                },
                croppedImageResizePolicy: (ref int width, ref int height) =>
                {
                    width = 256;
                    height = 256;
                });
        }

        public static Texture2D LoadAvatar(string filePath) {
 
            Texture2D tex = null;
            byte[] fileData;
 
            if (File.Exists(filePath))     {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }
        
        private IEnumerator LoadSaveImageContent(string url)
        {
            using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture2D = DownloadHandlerTexture.GetContent(webRequest);
                if (texture2D != null)
                {
                    _avatar.avatar.texture = texture2D;
                }
            }
        }

        protected override void Close()
        {
            base.Close();
            if (_isAgree && CheckChanges())
            {
                onCancelSaveClick?.Invoke(new UserInfo
                {
                    name = _changedName,
                    innerUserInfo = new InnerUserInfo
                    {
                        firstName = _changedFirstName,
                        lastName = _changedLastName,
                        email = _changedEmail,
                        phone = _changedPhone,
                        address = _changedAddress
                    }
                });
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.OnProfileUpdate.Del(OnProfileUpdated);
        }

        void OnProfileUpdated()
        {
            onRequestNewData?.Invoke();
            CheckChanges();
        }

        public void Init(UserInfo user)
        {
            _userData = user;
            _changedName = user.name;
            _changedAddress = user.innerUserInfo.address;
            _changedEmail = user.innerUserInfo.email;
            _changedPhone = user.innerUserInfo.phone;
            _changedFirstName = user.innerUserInfo.firstName;
            _changedLastName = user.innerUserInfo.lastName;
            _user.Init(user);
            //if (!user.guest)
            //{
                _publicUserName.SimpleSet(user.name);
                _firstNameInput.SimpleSet(user.innerUserInfo.firstName);
                _lastNameInput.SimpleSet(user.innerUserInfo.lastName);
                _emailInput.SimpleSet(user.innerUserInfo.email);
                _phoneInput.SimpleSet(user.innerUserInfo.phone);
                _adressText.text = user.innerUserInfo.address.Replace('|', ' ');
                _emptyAdressText.SetActive(string.IsNullOrEmpty(user.innerUserInfo.address));
            //}
        }

        public void ChangeAddress(string newAddress)
        {
            _changedAddress = newAddress;
            _adressText.text = newAddress.Replace('|', ' ');
            CheckChanges();
        }

        bool CheckChanges()
        {
            var isChange = false;
            
            if (_changedName != _userData.name)
            {
                isChange = true;
            }
            if (_changedFirstName != _userData.innerUserInfo.firstName)
            {
                isChange = true;
            }
            if (_changedLastName!= _userData.innerUserInfo.lastName)
            {
                isChange = true;
            }
            if (_changedEmail != _userData.innerUserInfo.email)
            {
                isChange = true;
            }
            if (_changedPhone != _userData.innerUserInfo.phone)
            {
                isChange = true;
            }
            if (_changedAddress != _userData.innerUserInfo.address)
            {
                isChange = true;
            }
            _saveBtn.gameObject.SetActive(isChange);
            return isChange;
        }
    }
}