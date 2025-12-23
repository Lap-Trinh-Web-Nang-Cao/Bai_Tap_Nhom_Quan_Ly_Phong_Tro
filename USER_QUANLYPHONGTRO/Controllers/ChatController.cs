using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.Chat;

namespace USER_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller quản lý Chat giữa Chủ Trọ và Người Thuê
    /// Sử dụng SignalR Hub từ Backend API (.NET 8)
    /// </summary>
    public class ChatController : Controller
    {
        private readonly string _apiBaseUrl;
        private readonly string _signalRHubUrl;

        public ChatController()
        {
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039/";
            _signalRHubUrl = ConfigurationManager.AppSettings["SignalRHubUrl"] ?? "http://localhost:7039/chatHub";
        }

        /// <summary>
        /// Trang Chat chung
        /// </summary>
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();
            var userName = Session["UserName"] as string ?? "Người dùng";
            var avatarUrl = Session["AvatarUrl"] as string ?? "/Content/images/default-avatar.png";
            var roleStr = Session["UserRole"] != null ? Session["UserRole"].ToString() : "";

            string role = "KhachThue";
            if (roleStr == "2" || roleStr.ToUpper() == "CHUTRO")
            {
                role = "ChuTro";
            }

            List<ContactViewModel> contacts;
            if (role == "ChuTro")
            {
                contacts = await GetContactsForLandlord(Guid.Parse(userId));
            }
            else
            {
                contacts = await GetContactsForTenant(Guid.Parse(userId));
            }

            var model = new ChatPageViewModel
            {
                CurrentUserId = userId,
                CurrentUserName = userName,
                CurrentUserRole = role,
                CurrentUserAvatar = avatarUrl,
                Contacts = contacts,
                SignalRHubUrl = _signalRHubUrl
            };

            return View(model);
        }

        /// <summary>
        /// Trang Chat cho Chủ Trọ
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ChuTro()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();
            var userName = Session["UserName"] as string ?? "Chủ trọ";
            var avatarUrl = Session["AvatarUrl"] as string ?? "/Content/images/default-avatar.png";

            var model = new ChatPageViewModel
            {
                CurrentUserId = userId,
                CurrentUserName = userName,
                CurrentUserRole = "ChuTro",
                CurrentUserAvatar = avatarUrl,
                Contacts = await GetContactsForLandlord(Guid.Parse(userId)),
                SignalRHubUrl = _signalRHubUrl
            };

            return View("Index", model);
        }

        /// <summary>
        /// Trang Chat cho Người Thuê
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> KhachThue()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();
            var userName = Session["UserName"] as string ?? "Khách thuê";
            var avatarUrl = Session["AvatarUrl"] as string ?? "/Content/images/default-avatar.png";

            var model = new ChatPageViewModel
            {
                CurrentUserId = userId,
                CurrentUserName = userName,
                CurrentUserRole = "KhachThue",
                CurrentUserAvatar = avatarUrl,
                Contacts = await GetContactsForTenant(Guid.Parse(userId)),
                SignalRHubUrl = _signalRHubUrl
            };

            return View("Index", model);
        }

        /// <summary>
        /// API lấy lịch sử tin nhắn
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetMessageHistory(string withUserId, int page = 1, int pageSize = 50)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            var currentUserId = Session["UserId"].ToString();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    var url = string.Format("api/chat/history?user1={0}&user2={1}&page={2}&pageSize={3}",
                        currentUserId, withUserId, page, pageSize);
                    
                    System.Diagnostics.Debug.WriteLine("📜 Getting chat history: " + url);
                    
                    var response = await client.GetAsync(url);
                    var json = await response.Content.ReadAsStringAsync();
                    
                    System.Diagnostics.Debug.WriteLine("📜 Response: " + json);

                    if (response.IsSuccessStatusCode)
                    {
                        var messages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChatMessageDto>>(json);
                        return Json(new { success = true, data = messages ?? new List<ChatMessageDto>() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ API Error: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ GetMessageHistory Error: " + ex.Message);
            }

            // Trả về rỗng nếu có lỗi
            return Json(new { success = true, data = new List<ChatMessageDto>() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API lưu tin nhắn vào database
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> SaveMessage(string toUserId, string content, string messageType)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            var fromUserId = Session["UserId"].ToString();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var payload = new
                    {
                        fromUserId = fromUserId,
                        toUserId = toUserId,
                        content = content,
                        messageType = messageType ?? "text"
                    };

                    var jsonContent = new StringContent(
                        Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                        System.Text.Encoding.UTF8,
                        "application/json");

                    System.Diagnostics.Debug.WriteLine("💾 Saving message: " + Newtonsoft.Json.JsonConvert.SerializeObject(payload));

                    var response = await client.PostAsync("api/chat/send", jsonContent);
                    var responseJson = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine("💾 Save response: " + responseJson);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, data = responseJson });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Save API Error: " + response.StatusCode + " - " + responseJson);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ SaveMessage Error: " + ex.Message);
            }

            return Json(new { success = true, message = "Tin nhắn đã gửi (realtime only)" });
        }

        /// <summary>
        /// API lấy danh sách liên hệ
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetContacts()
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            var userId = Guid.Parse(Session["UserId"].ToString());
            var roleStr = Session["UserRole"] != null ? Session["UserRole"].ToString() : "";

            List<ContactViewModel> contacts;
            if (roleStr == "2" || roleStr.ToUpper() == "CHUTRO")
            {
                contacts = await GetContactsForLandlord(userId);
            }
            else
            {
                contacts = await GetContactsForTenant(userId);
            }

            return Json(new { success = true, data = contacts }, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        /// <summary>
        /// Lấy danh sách liên hệ cho Chủ Trọ (Người thuê của họ)
        /// </summary>
        private async Task<List<ContactViewModel>> GetContactsForLandlord(Guid landlordId)
        {
            var contacts = new List<ContactViewModel>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    // Lấy danh sách người đã đặt phòng
                    var response = await client.GetAsync(string.Format("api/datphong/landlord-requests?userId={0}", landlordId));

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);

                        var addedUsers = new HashSet<string>();

                        if (bookings != null)
                        {
                            foreach (var booking in bookings)
                            {
                                if (booking.nguoiThue != null)
                                {
                                    string oderId = null;
                                    if (booking.nguoiThue.userId != null)
                                        oderId = booking.nguoiThue.userId.ToString();
                                    else if (booking.nguoiThue.UserId != null)
                                        oderId = booking.nguoiThue.UserId.ToString();
                                    
                                    if (!string.IsNullOrEmpty(oderId) && !addedUsers.Contains(oderId))
                                    {
                                        string tenKhach = "Khách thuê";
                                        if (booking.nguoiThue.hoSoNguoiDung != null && booking.nguoiThue.hoSoNguoiDung.hoTen != null)
                                            tenKhach = (string)booking.nguoiThue.hoSoNguoiDung.hoTen;
                                        else if (booking.nguoiThue.email != null)
                                            tenKhach = (string)booking.nguoiThue.email;

                                        contacts.Add(new ContactViewModel
                                        {
                                            UserId = oderId,
                                            UserName = tenKhach,
                                            Role = "KhachThue",
                                            AvatarUrl = "/Content/images/default-avatar.png",
                                            LastMessage = "Nhấn để bắt đầu trò chuyện",
                                            LastMessageTime = DateTime.Now
                                        });

                                        addedUsers.Add(oderId);
                                    }
                                }
                            }
                        }
                    }

                    // Lấy thêm danh sách từ tin nhắn cũ
                    await AddContactsFromChatHistory(contacts, landlordId.ToString(), client);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetContactsForLandlord Error: " + ex.Message);
            }

            return contacts;
        }

        /// <summary>
        /// Lấy danh sách liên hệ cho Người Thuê (Chủ trọ của họ)
        /// </summary>
        private async Task<List<ContactViewModel>> GetContactsForTenant(Guid tenantId)
        {
            var contacts = new List<ContactViewModel>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    var response = await client.GetAsync("api/datphong/my-bookings");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);

                        var addedUsers = new HashSet<string>();

                        if (bookings != null)
                        {
                            foreach (var booking in bookings)
                            {
                                if (booking.phong != null && booking.phong.nhaTro != null)
                                {
                                    string chuTroId = null;
                                    if (booking.phong.nhaTro.chuTroId != null)
                                        chuTroId = booking.phong.nhaTro.chuTroId.ToString();
                                    else if (booking.phong.nhaTro.ChuTroId != null)
                                        chuTroId = booking.phong.nhaTro.ChuTroId.ToString();
                                    
                                    if (!string.IsNullOrEmpty(chuTroId) && !addedUsers.Contains(chuTroId))
                                    {
                                        string tenChuTro = "Chủ trọ";
                                        if (booking.phong.nhaTro.tieuDe != null)
                                            tenChuTro = (string)booking.phong.nhaTro.tieuDe;
                                        else if (booking.phong.nhaTro.TieuDe != null)
                                            tenChuTro = (string)booking.phong.nhaTro.TieuDe;

                                        contacts.Add(new ContactViewModel
                                        {
                                            UserId = chuTroId,
                                            UserName = tenChuTro,
                                            Role = "ChuTro",
                                            AvatarUrl = "/Content/images/default-avatar.png",
                                            LastMessage = "Nhấn để bắt đầu trò chuyện",
                                            LastMessageTime = DateTime.Now
                                        });

                                        addedUsers.Add(chuTroId);
                                    }
                                }
                            }
                        }
                    }

                    // Lấy thêm danh sách từ tin nhắn cũ
                    await AddContactsFromChatHistory(contacts, tenantId.ToString(), client);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetContactsForTenant Error: " + ex.Message);
            }

            return contacts;
        }

        /// <summary>
        /// Thêm liên hệ từ lịch sử chat
        /// </summary>
        private async Task AddContactsFromChatHistory(List<ContactViewModel> contacts, string userId, HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(string.Format("api/chat/contacts?userId={0}", userId));
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var chatContacts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);

                    if (chatContacts != null)
                    {
                        var existingIds = new HashSet<string>(contacts.ConvertAll(c => c.UserId));

                        foreach (var contact in chatContacts)
                        {
                            string contactId = contact.userId?.ToString();
                            // ✅ FILTER: Không thêm chính người dùng hiện tại vào danh sách liên hệ
                            if (!string.IsNullOrEmpty(contactId) && 
                                !existingIds.Contains(contactId) && 
                                contactId != userId)  // 👈 Thêm check này
                            {
                                contacts.Add(new ContactViewModel
                                {
                                    UserId = contactId,
                                    UserName = contact.userName?.ToString() ?? "Người dùng",
                                    Role = "Unknown",
                                    AvatarUrl = contact.avatarUrl?.ToString() ?? "/Content/images/default-avatar.png",
                                    LastMessage = contact.lastMessage?.ToString() ?? "",
                                    LastMessageTime = contact.lastMessageTime != null ? 
                                        DateTime.Parse(contact.lastMessageTime.ToString()) : DateTime.Now
                                });
                                existingIds.Add(contactId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddContactsFromChatHistory Error: " + ex.Message);
            }
        }

        #endregion
    }

    #region DTOs

    public class ChatMessageDto
    {
        public string MessageId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }

    #endregion
}