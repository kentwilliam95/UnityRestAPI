using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

public class TrialRestAPI : MonoBehaviour
{
    private const string Token = "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";
    private const string URLOctocat = "https://api.github.com/octocat";
    private const string URLCurrentUser = "https://api.github.com/user";
    private const string URLCurrentUserRepositories = "https://api.github.com/user/repos";
    private const string URLSearchRepository = "https://api.github.com/search/repositories?";
    private const string TokenType = "Bearer";
    private StringBuilder stringBuilder;
    public struct RepoHeader
    {
        public RepoDetailHeader[] items;
    }

    public struct RepoDetailHeader
    {
        public string name;
        public OwnerDetail owner;
        public string full_name;
        public int stargazers_count;
    }

    public struct OwnerDetail
    {
        public string login;
    }

    private string _repoName;
    [SerializeField] private TextMeshProUGUI _textDebugLog;
    [SerializeField] private TMP_InputField _inputField;

    private void Start()
    {
        stringBuilder = new StringBuilder();
    }

    public void ButtonSearchByRepo_OnClick()
    {
        var repoName = _inputField.text;
        if (string.IsNullOrEmpty(repoName))
        {
            _textDebugLog.text = "Invalid Repo Name, insert repo name";
        }
        else
        {
            var query = "q={0}";
            var queryMod = string.Format(query, _inputField.text);
            var urlMod = string.Concat(URLSearchRepository, queryMod);

            _textDebugLog.text = "Sending Request";
            var uwr = APIController.SetupGetRequest(urlMod, TokenType, Token);
            APIController.Instance.StartSendRequest(uwr, SendSearchRepoRequest_OnComplete, SendSearchRepoRequest_OnFailed);
        }
    }

    public void ButtonSearchTop10VoxelRepo_OnClick()
    {
        var query = "q={0}{1}";
        query = String.Format(query, "voxel", "&sort=stars&order=desc&per_page=10");
        var queryMod = string.Concat(URLSearchRepository, query);
        
        _textDebugLog.text = "Sending Request";
        var uwr = APIController.SetupGetRequest(queryMod, TokenType, Token);        
        APIController.Instance.StartSendRequest(uwr, SendSearchRepoRequest_OnComplete, SendSearchRepoRequest_OnFailed);
    }

    public void ClearScreen()
    {
        _textDebugLog.text = string.Empty;
    }

    private void SendSearchRepoRequest_OnFailed(string result)
    {
        _textDebugLog.text = result;
    }

    private void SendSearchRepoRequest_OnComplete(string result)
    {
        var jsonResult = JsonConvert.DeserializeObject<RepoHeader>(result);
        stringBuilder.Clear();

        for (int i = 0; i < jsonResult.items.Length; i++)
        {
            var item = jsonResult.items[i];
            stringBuilder.AppendLine($"Repo Name : {item.name}");
            stringBuilder.AppendLine($"Full Name : {item.full_name}");
            stringBuilder.AppendLine($"Owner Name : {item.owner.login}");
            stringBuilder.AppendLine($"Stars : {item.stargazers_count}");
            stringBuilder.AppendLine("#####################################################");
            stringBuilder.AppendLine();
        }

        _textDebugLog.text = stringBuilder.ToString();
    }
}