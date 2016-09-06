using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.DTO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EdugameCloud.Lti.Tests
{
    public class DeserializationTests
    {
        [Test]
        public void DeserializeImageQuestionTest()
        {
            // not working \" < p > option A < img alt =\"\" height=\"73\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> \" : \"true\"
            // working: var response = "{\"answersList\": [{ \" < p > option A < img alt =\\\"\\\" height=\\\"73\\\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> \" : \"true\" }]}";
            //var response = "{\"instructions\": \"\", \"answersList\": [\"\"], \"description\": \"\", \"id\": \"98\", \"text\": \"<p>What is that?<img alt=\\\"\\\" height=\\\"200\\\" src=\\\"http://sakai11.esynctraining.com/access/content/group/test_lti/ubuntu-logo32.png\\\" width=\\\"200\\\" /></p> \", \"type\": \"Essay\"}";
            //var response = "{\"name\": \"Test images\", \"isAvailable\": \"true\", \"questions\": [{\"instructions\": \"\", \"answersList\": [{\"<p>option A </p> \": \"true\", \"imageBinary\": \"/9j//Z\", \"imageName\": \"<p>option A <img alt=\\\"\\\" height=\\\"73\\\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> .jpg\", , {\"<p>option B </p> \": \"false\", \"imageBinary\": \"/9j//9k=\"}]}\r\n";
            var response =
                "{\"name\":\"Test images\",\"isAvailable\":\"true\",\"questions\":[{\"answersList\":[{\"<p>option A </p> \":\"true\",\"imageBinary\":\"/9//Z\",\"imageName\":\"<p>option A <img alt=\\\"\\\" height=\\\"73\\\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> .jpg\",{\"<p>option B </p> \":\"false\",\"imageBinary\":\"/9j///2Q==\",\"imageName\":\"<p>option B <img alt=\\\"\\\" height=\\\"75\\\" src=\\\"https://upload.wikimedia.org/wikipedia/commons/c/c1/!!!_new_moon_!!!.jpg\\\" width=\\\"100\\\" /></p> .jpg\",],\"description\":\"\",\"id\":\"64\",\"text\":\"<p>Jpg image</p>  <p>Correct answer A</p>  <p></p>  <p> </p> \",\"type\":\"Multiple Choice\",\"questionImageBinary\":\"/9j//9k=\"}]}";
            //var response =
            //    "{\"name\":\"Test images\",\"isAvailable\":\"true\",\"questions\":[{\"instructions\":\"\",\"answersList\":[{\"<p>option A </p> \":\"true\",\"imageBinary\":\"/9//Z\",\"imageName\":\"<p>option A <img alt=\\\"\\\" height=\\\"73\\\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> .jpg\",,{\"<p>option B </p> \":\"false\",\"imageBinary\":\"/9j///2Q==\",\"imageName\":\"<p>option B <img alt=\\\"\\\" height=\\\"75\\\" src=\\\"https://upload.wikimedia.org/wikipedia/commons/c/c1/!!!_new_moon_!!!.jpg\\\" width=\\\"100\\\" /></p> .jpg\",],\"description\":\"\",\"id\":\"64\",\"text\":\"<p>Jpg image</p>  <p>Correct answer A</p>  <p></p>  <p> </p> \",\"type\":\"Multiple Choice\",\"questionImageBinary\":\"/9j//9k=\"}]}";
            //var response = "{\"answersList\": [{\"<p>option A <img alt=\\\"\\\" height=\\\"73\\\" src=\\\"http://hiddenuniversemovie.com/wp-content/uploads/2013/03/SUR8-SUN.NoGamut.tk006.left_1797_JPG-960x700.jpg\\\" width=\\\"100\\\" /></p> \" : \"true\"}]}";
            //var response = "{\"<p> option A <img alt =\"\" height=\"73\" /></p>\" : \"true\"}";
            JsonConvert.DeserializeObject<BBQuestionDTO>(response);

        }

        [Test]
        public void DeserializeMatchingQuestionTest()
        {
            var response = "{\"instructions\": \"Please match next\", \"answersList\": [{\"One\": \"111\",\"index\": \"0\"}, {\"Two\": \"222\",\"index\": \"1\"}, {\"Three\": \"333\",\"index\": \"2\"}, {\"<p><strong>Four (<span style=\\\"color: rgb(255,0,0);\\\">rich text</span>)</strong></p> \": \"<p><strong><span style=\\\"color: rgb(0,100,0);\\\"><em>444</em></span></strong></p> \",\"index\": \"3\"}], \"description\": \"\", \"id\": \"25\", \"text\": \"Please match next\", \"type\": \"Matching\"}";
            //var response = "{\"instructions\": \"\", \"answersList\": [\"\"], \"description\": \"\", \"id\": \"98\", \"text\": \"<p>What is that?<img alt=\\\\\"\\\\\" height=\\\\\"200\\\\\" src=\\\\\"http://sakai11.esynctraining.com/access/content/group/test_lti/ubuntu-logo32.png\\\\\" width=\\\\\"200\\\\\" /></p> \", \"type\": \"Essay\"}";
            JsonConvert.DeserializeObject<BBQuestionDTO>(response);

        }
    }
}
