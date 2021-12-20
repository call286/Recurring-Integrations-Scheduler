using Quartz;
using RecurringIntegrationsScheduler.Common.Contracts;
using RecurringIntegrationsScheduler.Properties;
using RecurringIntegrationsScheduler.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecurringIntegrationsScheduler.Forms
{
    public partial class CloneJob : Form
    {

        public bool Cancelled { get; private set; }
        public IJobDetail JobDetail { private get; set; }
        public ITrigger Trigger { private get; set; }
        public IJobDetail JobDetailNew { get; set; }
        public ITrigger TriggerNew { get; set; }

        public CloneJob()
        {
            InitializeComponent();
        }

        private void CloneJob_Load(object sender, EventArgs e)
        {
            textBoxNewName.Text = JobDetail.Key.Name + "-COPY";
            textBoxCompany.Text = JobDetail.JobDataMap.GetString(SettingsConstants.Company);
        }

        private void CloneJob_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                //FormsHelper.TrimTextBoxes(this);

                //var jobKey = new JobKey(textBoxNewName.Text, JobDetail.Key.Group);
                //if (Scheduler.Instance.GetScheduler().CheckExists(jobKey).Result)
                //    if (
                //        MessageBox.Show(
                //            string.Format(Resources.Job_0_in_group_1_already_exists, jobKey.Name, jobKey.Group),
                //            Resources.Job_already_exists, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                //        return;

                //if (!ValidateJobSettings()) return;
                //JobDetailNew = GetJobDetail();
                //TriggerNew = GetTrigger(JobDetailNew);
            }
        }
        private IJobDetail GetJobDetail()
        {
            var detail = JobBuilder
                .Create()
                .OfType(Type.GetType("RecurringIntegrationsScheduler.Job.Export,RecurringIntegrationsScheduler.Job.Export", true))
                .WithDescription(textBoxNewName.Text)
                .WithIdentity(new JobKey(textBoxNewName.Text, JobDetail.Key.Group))
                .UsingJobData(GetJobDataMap())
                .Build();

            return detail;
        }

        private JobDataMap GetJobDataMap()
        {
            var map = new JobDataMap
            {
                {SettingsConstants.DownloadSuccessDir, JobDetail.JobDataMap.GetString(SettingsConstants.DownloadSuccessDir)},
                {SettingsConstants.DownloadErrorsDir, JobDetail.JobDataMap.GetString(SettingsConstants.DownloadErrorsDir)},
                {SettingsConstants.AadTenant, JobDetail.JobDataMap.GetString(SettingsConstants.AadTenant)},
                {SettingsConstants.AzureAuthEndpoint, JobDetail.JobDataMap.GetString(SettingsConstants.AzureAuthEndpoint)},
                {SettingsConstants.AosUri, JobDetail.JobDataMap.GetString(SettingsConstants.AosUri)},
                {SettingsConstants.UseADAL, JobDetail.JobDataMap.GetString(SettingsConstants.UseADAL)},
                {SettingsConstants.UseServiceAuthentication, JobDetail.JobDataMap.GetString(SettingsConstants.UseServiceAuthentication)},
                {SettingsConstants.AadClientId, JobDetail.JobDataMap.GetString(SettingsConstants.AadClientId)},
                {SettingsConstants.UnzipPackage, JobDetail.JobDataMap.GetString(SettingsConstants.UnzipPackage)},
                {SettingsConstants.AddTimestamp, JobDetail.JobDataMap.GetString(SettingsConstants.AddTimestamp)},
                {SettingsConstants.DeletePackage, JobDetail.JobDataMap.GetString(SettingsConstants.DeletePackage)},
                {SettingsConstants.DataProject, JobDetail.JobDataMap.GetString(SettingsConstants.DataProject)},
                {SettingsConstants.Company, textBoxCompany.Text},
                {SettingsConstants.DelayBetweenFiles, JobDetail.JobDataMap.GetString(SettingsConstants.DelayBetweenFiles)},
                {SettingsConstants.DelayBetweenStatusCheck, JobDetail.JobDataMap.GetString(SettingsConstants.DelayBetweenStatusCheck)},
                {SettingsConstants.RetryCount, JobDetail.JobDataMap.GetString(SettingsConstants.RetryCount)},
                {SettingsConstants.RetryDelay, JobDetail.JobDataMap.GetString(SettingsConstants.RetryDelay)},
                {SettingsConstants.PauseJobOnException, JobDetail.JobDataMap.GetString(SettingsConstants.PauseJobOnException)},
                {SettingsConstants.ExportToPackageActionPath, JobDetail.JobDataMap.GetString(SettingsConstants.ExportToPackageActionPath)},
                {SettingsConstants.GetExecutionSummaryStatusActionPath, JobDetail.JobDataMap.GetString(SettingsConstants.GetExecutionSummaryStatusActionPath)},
                {SettingsConstants.GetExportedPackageUrlActionPath, JobDetail.JobDataMap.GetString(SettingsConstants.GetExportedPackageUrlActionPath)},
                {SettingsConstants.IndefinitePause, JobDetail.JobDataMap.GetString(SettingsConstants.IndefinitePause)},
                {SettingsConstants.LogVerbose, JobDetail.JobDataMap.GetString(SettingsConstants.LogVerbose)}
            };
            if (JobDetail.JobDataMap.GetString(SettingsConstants.UseServiceAuthentication)=="true")
            {
                map.Add(SettingsConstants.AadClientSecret, JobDetail.JobDataMap.GetString(SettingsConstants.AadClientSecret));
            }
            else
            {
                map.Add(SettingsConstants.UserName, JobDetail.JobDataMap.GetString(SettingsConstants.UserName));
                map.Add(SettingsConstants.UserPassword, JobDetail.JobDataMap.GetString(SettingsConstants.UserPassword));
            }
            return map;
        }

        private ITrigger GetTrigger(IJobDetail jobDetail)
        {
            var builder = Trigger.GetTriggerBuilder().ForJob(jobDetail);
            //TriggerBuilder
            //    .Create()
            //    .ForJob(jobDetail)
            //    .WithDescription(string.Format(Resources.Trigger_for_job_0_1, jobDetail.Key.Name,
            //        jobDetail.Key.Group))
            //    .WithIdentity(
            //        new TriggerKey(
            //            string.Format(Resources.Trigger_for_job_0_1, jobDetail.Key.Name, jobDetail.Key.Group),
            //            jobDetail.Key.Group));
            return builder.Build();
        }


        private bool ValidateJobSettings()
        {
            var message = new StringBuilder();

            if (string.IsNullOrEmpty(textBoxNewName.Text))
                message.AppendLine(Resources.Job_name_is_missing);

            if (string.IsNullOrEmpty(JobDetail.Key.Group))
                message.AppendLine(Resources.Job_group_is_not_selected);

            if (string.IsNullOrEmpty(textBoxCompany.Text))
                message.AppendLine(Resources.Legal_entity_is_missing);

            if (message.Length > 0)
                MessageBox.Show(message.ToString(), Resources.Job_configuration_is_not_valid);

            return message.Length == 0;
        }
    }
}
