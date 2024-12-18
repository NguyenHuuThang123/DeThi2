using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeThi2.Models;

namespace DeThi2
{
    public partial class frmSanPham : Form
    {
       

        public frmSanPham()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 sp = new Model1();
                List<LoaiSP> listLoai = sp.LoaiSPs.ToList();
                List<Sanpham> listSanpham = sp.Sanphams.ToList();
                FillFalcultyCombobox(listLoai);
                BindGrid(listSanpham);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void FillFalcultyCombobox(List<LoaiSP> listLoais)
        {
            this.cboLoaiSP.DataSource = listLoais;
            this.cboLoaiSP.DisplayMember = "TenLoai";
            this.cboLoaiSP.ValueMember = "MaLoai";
        }

        private void BindGrid(List<Sanpham> listStudent)
        {
            dvgSanPham.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dvgSanPham.Rows.Add();
                dvgSanPham.Rows[index].Cells[0].Value = item.MaSP;
                dvgSanPham.Rows[index].Cells[1].Value = item.TenSP;
                dvgSanPham.Rows[index].Cells[2].Value = item.Ngaynhap;
                dvgSanPham.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 sp = new Model1())
                {
                    var existingProduct = sp.Sanphams.FirstOrDefault(s => s.MaSP == txtMaSP.Text);

                    if (existingProduct == null) // Adding a new product
                    {
                        var newProduct = new Sanpham
                        {
                            MaSP = txtMaSP.Text,
                            TenSP = txtTenSP.Text,
                            Ngaynhap = dtNgaynhap.Value,
                            MaLoai = cboLoaiSP.SelectedValue?.ToString()
                        };

                        sp.Sanphams.Add(newProduct);
                    }
                    else // Editing an existing product
                    {
                        existingProduct.TenSP = txtTenSP.Text;
                        existingProduct.Ngaynhap = dtNgaynhap.Value;
                        existingProduct.MaLoai = cboLoaiSP.SelectedValue?.ToString();
                    }

                    sp.SaveChanges();
                    BindGrid(sp.Sanphams.ToList());

                    MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                btnLuu.Enabled = false;
                btnKLuu.Enabled = false;
                txtMaSP.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            btnKLuu.Enabled=true;
            btnLuu.Enabled=true;
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            // Reset input fields
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;

            btnLuu.Enabled = false;
            btnKLuu.Enabled = false;
            txtMaSP.Enabled = true;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            btnKLuu.Enabled = true;
            btnLuu.Enabled = true;
        }

        private void dvgSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dvgSanPham.Rows[e.RowIndex]; 
                txtMaSP.Text = row.Cells[0].Value.ToString();
                txtTenSP.Text = row.Cells[1].Value.ToString();
                dtNgaynhap.Text = row.Cells[2].Value.ToString();
                cboLoaiSP.Text = row.Cells[3].Value.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 db = new Model1();
                List<Sanpham> sanphamList = db.Sanphams.ToList();

                var sanpham = sanphamList.FirstOrDefault(s => s.MaSP == txtMaSP.Text);
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dòng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    if (sanpham != null)
                    {
                   
                        db.Sanphams.Remove(sanpham);
                        db.SaveChanges();

                        BindGrid(db.Sanphams.ToList());

                        MessageBox.Show("Sản phẩm đã được xoá thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sản phảm không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                 }
                catch (Exception ex)
                {
            }
            MessageBox.Show($"Lỗi khi cập nhật dữ liệu: [ex.Message)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchMaSP = txtTim.Text.Trim();

                using (Model1 sp = new Model1())
                {
                    // Tìm kiếm sản phẩm theo Mã SP
                    var result = sp.Sanphams
                                  .Where(s => s.MaSP.Contains(searchMaSP))
                                  .ToList();

                    // Hiển thị kết quả trong DataGridView
                    if (result.Any())
                    {
                        BindGrid(result);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm nào có Mã SP như vậy!", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Xóa dữ liệu trên DataGridView khi không tìm thấy
                        dvgSanPham.Rows.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đóng chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
