namespace VLXD_API.Handlers
{
    public class BamMatKhauHandler
    {
        /// <summary>
        /// Băm mật khẩu sử dụng BCrypt
        /// </summary>
        public static string PasswordHash(string password)
        {
            // BCrypt tự động tạo Salt và tích hợp vào chuỗi Hash kết quả.
            // 'workFactor' càng cao thì càng tốn thời gian tính toán (mặc định thường là 11 hoặc 12).
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Kiểm tra mật khẩu nhập vào với chuỗi Hash đã lưu
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // BCrypt.Verify sẽ tự tách Salt từ chuỗi hashedPassword để kiểm tra
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception)
            {
                // Trả về false nếu định dạng hash không hợp lệ hoặc có lỗi xảy ra
                return false;
            }
        }
    }
}
