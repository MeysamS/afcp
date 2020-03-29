var g_days = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31], j_days = [31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29];
function gregorianToJalali(g_y, g_m, g_d) {
    g_y = parseInt(g_y);
    g_m = parseInt(g_m);
    g_d = parseInt(g_d);
    var gy = g_y - 1600;
    var gm = g_m - 1;
    var gd = g_d - 1;
    var g_day_no = 365 * gy + parseInt((gy + 3) / 4) - parseInt((gy + 99) / 100) + parseInt((gy + 399) / 400);
    for (var i = 0; i < gm; ++i)
        g_day_no += g_days[i];
    if (gm > 1 && ((gy % 4 == 0 && gy % 100 != 0) || (gy % 400 == 0)))
        ++g_day_no;
    g_day_no += gd;
    var j_day_no = g_day_no - 79;
    var j_np = parseInt(j_day_no / 12053);
    j_day_no %= 12053;
    var jy = 979 + 33 * j_np + 4 * parseInt(j_day_no / 1461);
    j_day_no %= 1461;
    if (j_day_no >= 366) {
        jy += parseInt((j_day_no - 1) / 365);
        j_day_no = (j_day_no - 1) % 365;
    }
    for (var i = 0; i < 11 && j_day_no >= j_days[i]; ++i)
        j_day_no -= j_days[i];
    var jm = i + 1;
    var jd = j_day_no + 1;
    return [jy, jm, jd];
}