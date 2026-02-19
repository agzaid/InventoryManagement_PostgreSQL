const _AlertHelper = {
    // 1. رسالة نجاح
    showSuccess: function (message, title = "تم بنجاح") {
        return Swal.fire({
            icon: 'success',
            title: title,
            text: message,
            confirmButtonText: 'موافق',
            timer: 2500,
            timerProgressBar: true,
            confirmButtonColor: '#0ea5e9' // لون Primary الخاص بك
        });
    },

    // 2. رسالة خطأ
    showError: function (message, title = "خطأ!") {
        return Swal.fire({
            icon: 'error',
            title: title,
            text: message,
            confirmButtonText: 'موافق',
            confirmButtonColor: '#ef4444'
        });
    },

    // 2.b رسالة تحذير
    showWarning: function (message, title = "تنبيه") {
        return Swal.fire({
            icon: 'warning',
            title: title,
            text: message,
            confirmButtonText: 'موافق',
            confirmButtonColor: '#f59e0b'
        });
    },

    // 3. رسالة تأكيد (لحذف أو حفظ)
    showConfirm: function (title, text, confirmText = "نعم، استمر") {
        return Swal.fire({
            icon: 'question',
            title: title,
            text: text,
            showCancelButton: true,
            confirmButtonText: confirmText,
            cancelButtonText: 'إلغاء',
            confirmButtonColor: '#0ea5e9',
            cancelButtonColor: '#64748b'
        });
    },

    // 4. رسالة صغيرة (Toast) تظهر في الزاوية وتختفي
    showToast: function (message, icon = 'success') {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        Toast.fire({
            icon: icon,
            title: message
        });
    },
    showLoading: function () {
        Swal.fire({
            title: 'جاري الحفظ...',
            didOpen: () => { Swal.showLoading(); },
            allowOutsideClick: false
        });
    }
};