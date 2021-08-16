//  AddressableAssetAddressClassCreator.cs
//  http://kan-kikuchi.hatenablog.com/entry/AddressableAssetAddressClassCreator
//
//  Created by kan.kikuchi on 2019.09.16.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// �萔���Ǘ�����N���X�𐶐�����N���X
/// </summary>
public static class ConstantsClassCreator
{

    //�����ȕ������Ǘ�����z��
    private static readonly string[] INVALUD_CHARS = {
    " ", "!", "\"", "#", "$",
    "%", "&", "\'", "(", ")",
    "-", "=", "^",  "~", "\\",
    "|", "[", "{",  "@", "`",
    "]", "}", ":",  "*", ";",
    "+", "/", "?",  ".", ">",
    ",", "<"
  };

    //�萔�̋�؂蕶��
    private const char DELIMITER = '_';

    //�^��
    private const string STRING_NAME = "string";
    private const string INT_NAME = "int";
    private const string FLOAT_NAME = "float";

    /// <summary>
    /// �萔���Ǘ�����N���X��������������
    /// </summary>
    public static void Create<T>(string className, string classInfo, string directoryPath, Dictionary<string, T> valueDict)
    {
        //���͂��ꂽ�^�̔���
        string typeName = null;

        if (typeof(T) == typeof(string))
        {
            typeName = STRING_NAME;
        }
        else if (typeof(T) == typeof(int))
        {
            typeName = INT_NAME;
        }
        else if (typeof(T) == typeof(float))
        {
            typeName = FLOAT_NAME;
        }
        else
        {
            Debug.Log(className + "�̍쐬�Ɏ��s���܂���.�z��O�̌^" + typeof(T).Name + "�����͂���܂���");
            return;
        }

        //�f�B�N�V���i���[���\�[�g�������̂�
        SortedDictionary<string, T> sortDict = new SortedDictionary<string, T>(valueDict);

        //���͂��ꂽ������key���疳���ȕ�������폜���āA�啶����_��ݒ肵���萔���Ɠ������̂ɕύX���V���Ȏ����ɓo�^
        //���̒萔�̍ő咷���߂�Ƃ���ŁA_���܂߂����̂��擾�������̂ŁA��Ɏ��s
        Dictionary<string, T> newValueDict = new Dictionary<string, T>();

        foreach (KeyValuePair<string, T> valuePair in sortDict)
        {
            string newKey = RemoveInvalidChars(valuePair.Key);
            newKey = SetDelimiterBeforeUppercase(newKey);
            newValueDict[newKey] = valuePair.Value;
        }

        //�萔���̍ő咷���擾���A�󔒐�������
        int keyLengthMax = 0;
        if (newValueDict.Count > 0)
        {
            keyLengthMax = 1 + newValueDict.Keys.Select(key => key.Length).Max();
        }

        //�R�����g���ƃN���X�������
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("/// <summary>");
        builder.AppendFormat("/// {0}", classInfo).AppendLine();
        builder.AppendLine("/// </summary>");
        builder.AppendFormat("public static class {0}", className).AppendLine("{").AppendLine();

        //���͂��ꂽ�萔�Ƃ��̒l�̃y�A�������o���Ă���
        string[] keyArray = newValueDict.Keys.ToArray();
        foreach (string key in keyArray)
        {

            if (string.IsNullOrEmpty(key))
            {
                continue;
            }
            //����������key��������X���[
            else if (System.Text.RegularExpressions.Regex.IsMatch(key, @"^[0-9]+$"))
            {
                continue;
            }
            //key�ɔ��p������_�ȊO���܂܂�Ă�����X���[
            else if (!System.Text.RegularExpressions.Regex.IsMatch(key, @"^[_a-zA-Z0-9]+$"))
            {
                continue;
            }

            //�C�R�[�������ԗp�ɋ󔒂𒲐�����
            string EqualStr = String.Format("{0, " + (keyLengthMax - key.Length).ToString() + "}", "=");

            //��L�Ŕ��肵���^�ƒ萔�������
            builder.Append("\t").AppendFormat(@"  public const {0} {1} {2} ", typeName, key, EqualStr);

            //T��string�̏ꍇ�͒l�̑O���"��t����
            if (typeName == STRING_NAME)
            {
                builder.AppendFormat(@"""{0}"";", newValueDict[key]).AppendLine();
            }

            //T��float�̏ꍇ�͒l�̌��f��t����
            else if (typeName == FLOAT_NAME)
            {
                builder.AppendFormat(@"{0}f;", newValueDict[key]).AppendLine();
            }

            else
            {
                builder.AppendFormat(@"{0};", newValueDict[key]).AppendLine();
            }

        }

        builder.AppendLine().AppendLine("}");

        //�����o���A�t�@�C�����̓N���X��.cs
        string exportPath = Path.Combine(directoryPath, className + ".cs");

        //�����o����̃f�B���N�g����������΍쐬
        string directoryName = Path.GetDirectoryName(exportPath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(exportPath, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        Debug.Log(className + "�̍쐬���������܂���");
    }


    /// <summary>
    /// �����ȕ������폜���܂�
    /// </summary>
    private static string RemoveInvalidChars(string str)
    {
        Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }

    /// <summary>
    /// ��؂蕶����啶���̑O�ɐݒ肷��
    /// </summary>
    private static string SetDelimiterBeforeUppercase(string str)
    {
        string conversionStr = "";


        for (int strNo = 0; strNo < str.Length; strNo++)
        {

            bool isSetDelimiter = true;

            //�ŏ��ɂ͐ݒ肵�Ȃ�
            if (strNo == 0)
            {
                isSetDelimiter = false;
            }
            //�������������Ȃ�ݒ肵�Ȃ�
            else if (char.IsLower(str[strNo]) || char.IsNumber(str[strNo]))
            {
                isSetDelimiter = false;
            }
            //���肵�Ă�̑O���啶���Ȃ�ݒ肵�Ȃ�(�A���啶���̎�)
            else if (char.IsUpper(str[strNo - 1]) && !char.IsNumber(str[strNo]))
            {
                isSetDelimiter = false;
            }
            //���肵�Ă镶�������̕����̑O����؂蕶���Ȃ�ݒ肵�Ȃ�
            else if (str[strNo] == DELIMITER || str[strNo - 1] == DELIMITER)
            {
                isSetDelimiter = false;
            }

            //�����ݒ�
            if (isSetDelimiter)
            {
                conversionStr += DELIMITER.ToString();
            }
            conversionStr += str.ToUpper()[strNo];

        }

        return conversionStr;
    }

}