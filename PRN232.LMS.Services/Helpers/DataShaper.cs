using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace PRN232.LMS.Services.Helpers
{
    public static class DataShaper
    {
        public static ExpandoObject ShapeData<T>(T entity, string? fieldsString)
        {
            var shapedObject = new ExpandoObject();
            var shapedDictionary = (IDictionary<string, object?>) shapedObject;

            // Lấy danh sách tất cả các thuộc tính Public của DTO
            var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Nếu Admin không truyền tham số fields hoặc truyền chuỗi trống, bốc hết tất cả các trường
            if (string.IsNullOrWhiteSpace(fieldsString))
            {
                foreach (var property in allProperties)
                {
                    shapedDictionary[property.Name] = property.GetValue(entity);
                }
                return shapedObject;
            }

            // Tách chuỗi "studentId,fullName" thành mảng các trường cần lấy
            var requiredFields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(f => f.Trim());

            foreach (var field in requiredFields)
            {
                // Dùng Reflection để tìm thuộc tính khớp tên (không phân biệt hoa thường)
                var property = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (property != null)
                {
                    // Đổ tên thuộc tính (giữ nguyên quy tắc viết hoa của DTO) và giá trị vào đối tượng động
                    shapedDictionary[property.Name] = property.GetValue(entity);
                }
            }

            return shapedObject;
        }
    }
}