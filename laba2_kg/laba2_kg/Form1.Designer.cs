namespace laba2_kg
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        //#region Код, автоматически созданный конструктором форм Windows

        ///// <summary>
        ///// Требуемый метод для поддержки конструктора — не изменяйте 
        ///// содержимое этого метода с помощью редактора кода.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //    this.glControl2 = new OpenTK.GLControl();
        //    this.SuspendLayout();
        //    // 
        //    // glControl2
        //    // 
        //    this.glControl2.BackColor = System.Drawing.Color.Black;
        //    this.glControl2.Location = new System.Drawing.Point(132, 79);
        //    this.glControl2.Name = "glControl2";
        //    this.glControl2.Size = new System.Drawing.Size(470, 306);
        //    this.glControl2.TabIndex = 0;
        //    this.glControl2.VSync = false;
        //    // 
        //    // Form1
        //    // 
        //    this.ClientSize = new System.Drawing.Size(752, 473);
        //    this.Controls.Add(this.glControl2);
        //    this.Name = "Form1";
        //    this.ResumeLayout(false);

        //}

        //#endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private OpenTK.GLControl glControl3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar3;
    }
}

