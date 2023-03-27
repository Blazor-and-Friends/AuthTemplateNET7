namespace AuthTemplateNET7.Server.Seeding;

//added

/// <summary>
/// See <see href="https://mockaroo.com/"/>
/// </summary>
public class SeederServices
{
    private readonly Random random;

    //courtesy https://mockaroo.com/
    string jsonFirstnameLastnameEmail = @"[{""Firstname"":""Ambrosius"",""Lastname"":""Ravilious"",""Email"":""aravilious0@chicagotribune.com""},
{""Firstname"":""Carmen"",""Lastname"":""Sparkwill"",""Email"":""csparkwill1@wsj.com""},
{""Firstname"":""Phylis"",""Lastname"":""Baraja"",""Email"":""pbaraja2@biblegateway.com""},
{""Firstname"":""Lin"",""Lastname"":""Batterham"",""Email"":""lbatterham3@ihg.com""},
{""Firstname"":""Jordain"",""Lastname"":""Sally"",""Email"":""jsally4@dot.gov""},
{""Firstname"":""Juieta"",""Lastname"":""De Minico"",""Email"":""jdeminico5@bbc.co.uk""},
{""Firstname"":""Elsi"",""Lastname"":""Ainsbury"",""Email"":""eainsbury6@digg.com""},
{""Firstname"":""Ranice"",""Lastname"":""Petrecz"",""Email"":""rpetrecz7@trellian.com""},
{""Firstname"":""Blisse"",""Lastname"":""Biesty"",""Email"":""bbiesty8@accuweather.com""},
{""Firstname"":""Michel"",""Lastname"":""O'Rudden"",""Email"":""morudden9@sitemeter.com""},
{""Firstname"":""Woodman"",""Lastname"":""Kalf"",""Email"":""wkalfa@usa.gov""},
{""Firstname"":""Beth"",""Lastname"":""Czapla"",""Email"":""bczaplab@guardian.co.uk""},
{""Firstname"":""Findley"",""Lastname"":""Savege"",""Email"":""fsavegec@forbes.com""},
{""Firstname"":""Alidia"",""Lastname"":""Livezley"",""Email"":""alivezleyd@homestead.com""},
{""Firstname"":""Vittorio"",""Lastname"":""Maylam"",""Email"":""vmaylame@boston.com""},
{""Firstname"":""Daisie"",""Lastname"":""Wheal"",""Email"":""dwhealf@seesaa.net""},
{""Firstname"":""Morie"",""Lastname"":""Zimmermanns"",""Email"":""mzimmermannsg@hao123.com""},
{""Firstname"":""Roxanne"",""Lastname"":""Stopher"",""Email"":""rstopherh@biblegateway.com""},
{""Firstname"":""Hilliard"",""Lastname"":""Petrasso"",""Email"":""hpetrassoi@gnu.org""},
{""Firstname"":""Calhoun"",""Lastname"":""Baulcombe"",""Email"":""cbaulcombej@mediafire.com""},
{""Firstname"":""Paul"",""Lastname"":""Winston"",""Email"":""pwinstonk@answers.com""},
{""Firstname"":""Lindy"",""Lastname"":""Elsbury"",""Email"":""lelsburyl@skype.com""},
{""Firstname"":""Dorey"",""Lastname"":""Howden"",""Email"":""dhowdenm@geocities.jp""},
{""Firstname"":""Zechariah"",""Lastname"":""Palatini"",""Email"":""zpalatinin@163.com""},
{""Firstname"":""Marjy"",""Lastname"":""Matzl"",""Email"":""mmatzlo@usa.gov""},
{""Firstname"":""Sissy"",""Lastname"":""Edison"",""Email"":""sedisonp@cyberchimps.com""},
{""Firstname"":""Elmer"",""Lastname"":""Mapstone"",""Email"":""emapstoneq@webmd.com""},
{""Firstname"":""Caritta"",""Lastname"":""Dunford"",""Email"":""cdunfordr@feedburner.com""},
{""Firstname"":""Sascha"",""Lastname"":""Gaitley"",""Email"":""sgaitleys@ucoz.ru""},
{""Firstname"":""Perla"",""Lastname"":""Getten"",""Email"":""pgettent@bizjournals.com""},
{""Firstname"":""Sally"",""Lastname"":""Raynor"",""Email"":""sraynoru@infoseek.co.jp""},
{""Firstname"":""Melina"",""Lastname"":""Castillon"",""Email"":""mcastillonv@taobao.com""},
{""Firstname"":""Merla"",""Lastname"":""Ricardot"",""Email"":""mricardotw@geocities.com""},
{""Firstname"":""Anatola"",""Lastname"":""Sawart"",""Email"":""asawartx@oaic.gov.au""},
{""Firstname"":""Matthew"",""Lastname"":""Sheal"",""Email"":""mshealy@issuu.com""},
{""Firstname"":""Jed"",""Lastname"":""Fardell"",""Email"":""jfardellz@wp.com""},
{""Firstname"":""Valera"",""Lastname"":""Guarin"",""Email"":""vguarin10@irs.gov""},
{""Firstname"":""Mariette"",""Lastname"":""Braddon"",""Email"":""mbraddon11@hao123.com""},
{""Firstname"":""Arlana"",""Lastname"":""Diggle"",""Email"":""adiggle12@dailymotion.com""},
{""Firstname"":""Morrie"",""Lastname"":""Eccleston"",""Email"":""meccleston13@wired.com""},
{""Firstname"":""Sophi"",""Lastname"":""Bucknall"",""Email"":""sbucknall14@tripadvisor.com""},
{""Firstname"":""Gerik"",""Lastname"":""Enderby"",""Email"":""genderby15@goodreads.com""},
{""Firstname"":""Cosme"",""Lastname"":""Jaycock"",""Email"":""cjaycock16@1und1.de""},
{""Firstname"":""Michelle"",""Lastname"":""Renton"",""Email"":""mrenton17@surveymonkey.com""},
{""Firstname"":""Natal"",""Lastname"":""Reagan"",""Email"":""nreagan18@bravesites.com""},
{""Firstname"":""Alaine"",""Lastname"":""Barnby"",""Email"":""abarnby19@unicef.org""},
{""Firstname"":""Brod"",""Lastname"":""Cheson"",""Email"":""bcheson1a@ebay.co.uk""},
{""Firstname"":""Benedick"",""Lastname"":""MacMillan"",""Email"":""bmacmillan1b@yahoo.co.jp""},
{""Firstname"":""Lotta"",""Lastname"":""Priver"",""Email"":""lpriver1c@webeden.co.uk""},
{""Firstname"":""Jess"",""Lastname"":""Cheyne"",""Email"":""jcheyne1d@jalbum.net""},
{""Firstname"":""Lurlene"",""Lastname"":""Vanyushkin"",""Email"":""lvanyushkin1e@naver.com""},
{""Firstname"":""Ondrea"",""Lastname"":""Joynson"",""Email"":""ojoynson1f@jimdo.com""},
{""Firstname"":""Andras"",""Lastname"":""Ivel"",""Email"":""aivel1g@unc.edu""},
{""Firstname"":""Marietta"",""Lastname"":""Cavil"",""Email"":""mcavil1h@github.com""},
{""Firstname"":""Yevette"",""Lastname"":""Antczak"",""Email"":""yantczak1i@youtu.be""},
{""Firstname"":""Salli"",""Lastname"":""Lansdowne"",""Email"":""slansdowne1j@gizmodo.com""},
{""Firstname"":""Danila"",""Lastname"":""Bugg"",""Email"":""dbugg1k@mac.com""},
{""Firstname"":""Melisa"",""Lastname"":""Sheekey"",""Email"":""msheekey1l@cloudflare.com""},
{""Firstname"":""Kay"",""Lastname"":""Grosvenor"",""Email"":""kgrosvenor1m@mit.edu""},
{""Firstname"":""Cammy"",""Lastname"":""Pott"",""Email"":""cpott1n@booking.com""},
{""Firstname"":""Trevor"",""Lastname"":""Satcher"",""Email"":""tsatcher1o@vinaora.com""},
{""Firstname"":""Oren"",""Lastname"":""Benettini"",""Email"":""obenettini1p@issuu.com""},
{""Firstname"":""Garrick"",""Lastname"":""McCoole"",""Email"":""gmccoole1q@gnu.org""},
{""Firstname"":""Brady"",""Lastname"":""Binding"",""Email"":""bbinding1r@alibaba.com""},
{""Firstname"":""Amelita"",""Lastname"":""Trippett"",""Email"":""atrippett1s@paypal.com""},
{""Firstname"":""Gaylene"",""Lastname"":""Portt"",""Email"":""gportt1t@nhs.uk""},
{""Firstname"":""Ashla"",""Lastname"":""Petrishchev"",""Email"":""apetrishchev1u@bravesites.com""},
{""Firstname"":""Petey"",""Lastname"":""Pettifer"",""Email"":""ppettifer1v@xinhuanet.com""},
{""Firstname"":""Evelyn"",""Lastname"":""Harber"",""Email"":""eharber1w@msn.com""},
{""Firstname"":""Clemens"",""Lastname"":""Bayns"",""Email"":""cbayns1x@auda.org.au""},
{""Firstname"":""Kaela"",""Lastname"":""Preuvost"",""Email"":""kpreuvost1y@creativecommons.org""},
{""Firstname"":""Correna"",""Lastname"":""Weale"",""Email"":""cweale1z@seesaa.net""},
{""Firstname"":""Shawnee"",""Lastname"":""Kershaw"",""Email"":""skershaw20@w3.org""},
{""Firstname"":""Marcelle"",""Lastname"":""Bakhrushin"",""Email"":""mbakhrushin21@auda.org.au""},
{""Firstname"":""Rog"",""Lastname"":""Carryer"",""Email"":""rcarryer22@buzzfeed.com""},
{""Firstname"":""Cristionna"",""Lastname"":""Careswell"",""Email"":""ccareswell23@google.com""},
{""Firstname"":""Wood"",""Lastname"":""Llewelyn"",""Email"":""wllewelyn24@mediafire.com""},
{""Firstname"":""Hal"",""Lastname"":""Allitt"",""Email"":""hallitt25@ebay.co.uk""},
{""Firstname"":""Almire"",""Lastname"":""Aucutt"",""Email"":""aaucutt26@google.co.jp""},
{""Firstname"":""Seumas"",""Lastname"":""Perkis"",""Email"":""sperkis27@freewebs.com""},
{""Firstname"":""Vanni"",""Lastname"":""Renals"",""Email"":""vrenals28@google.fr""},
{""Firstname"":""Alissa"",""Lastname"":""Vicarey"",""Email"":""avicarey29@elpais.com""},
{""Firstname"":""Crystie"",""Lastname"":""Kirkpatrick"",""Email"":""ckirkpatrick2a@rediff.com""},
{""Firstname"":""Stephen"",""Lastname"":""Linacre"",""Email"":""slinacre2b@last.fm""},
{""Firstname"":""Sasha"",""Lastname"":""Muddle"",""Email"":""smuddle2c@uiuc.edu""},
{""Firstname"":""Osmond"",""Lastname"":""Van Halen"",""Email"":""ovanhalen2d@cocolog-nifty.com""},
{""Firstname"":""Minnie"",""Lastname"":""Albone"",""Email"":""malbone2e@spotify.com""},
{""Firstname"":""Camila"",""Lastname"":""Cancott"",""Email"":""ccancott2f@ebay.com""},
{""Firstname"":""Rickie"",""Lastname"":""Swyer"",""Email"":""rswyer2g@addtoany.com""},
{""Firstname"":""Ollie"",""Lastname"":""Gatrell"",""Email"":""ogatrell2h@ameblo.jp""},
{""Firstname"":""Audi"",""Lastname"":""Barthram"",""Email"":""abarthram2i@discovery.com""},
{""Firstname"":""Wilow"",""Lastname"":""Fordyce"",""Email"":""wfordyce2j@tuttocitta.it""},
{""Firstname"":""Edsel"",""Lastname"":""Nowick"",""Email"":""enowick2k@wiley.com""},
{""Firstname"":""Mallissa"",""Lastname"":""Trainer"",""Email"":""mtrainer2l@free.fr""},
{""Firstname"":""Lexy"",""Lastname"":""McCromley"",""Email"":""lmccromley2m@globo.com""},
{""Firstname"":""Wynny"",""Lastname"":""Barlow"",""Email"":""wbarlow2n@mit.edu""},
{""Firstname"":""Luise"",""Lastname"":""Gallahue"",""Email"":""lgallahue2o@answers.com""},
{""Firstname"":""Gavin"",""Lastname"":""De La Haye"",""Email"":""gdelahaye2p@issuu.com""},
{""Firstname"":""Crysta"",""Lastname"":""Losel"",""Email"":""closel2q@mozilla.org""},
{""Firstname"":""Evangelin"",""Lastname"":""Chezelle"",""Email"":""echezelle2r@imdb.com""}]";
    string jsonSentences = @"[{""Lorem"":""Nam dui. Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo.""},
{""Lorem"":""Fusce posuere felis sed lacus. Morbi sem mauris, laoreet ut, rhoncus aliquet, pulvinar sed, nisl. Nunc rhoncus dui vel sem. Sed sagittis. Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus. Pellentesque at nulla.""},
{""Lorem"":""In eleifend quam a odio. In hac habitasse platea dictumst. Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem. Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst. Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem.""},
{""Lorem"":""Vivamus tortor. Duis mattis egestas metus. Aenean fermentum.""},
{""Lorem"":""Fusce congue, diam id ornare imperdiet, sapien urna pretium nisl, ut volutpat sapien arcu sed augue. Aliquam erat volutpat. In congue. Etiam justo.""},
{""Lorem"":""Etiam faucibus cursus urna. Ut tellus.""},
{""Lorem"":""Quisque arcu libero, rutrum ac, lobortis vel, dapibus at, diam. Nam tristique tortor eu pede.""},
{""Lorem"":""Morbi porttitor lorem id ligula. Suspendisse ornare consequat lectus. In est risus, auctor sed, tristique in, tempus sit amet, sem. Fusce consequat. Nulla nisl. Nunc nisl.""},
{""Lorem"":""Morbi a ipsum.""},
{""Lorem"":""Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo. Pellentesque viverra pede ac diam.""},
{""Lorem"":""Nulla mollis molestie lorem. Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst. Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem. Integer tincidunt ante vel ipsum. Praesent blandit lacinia erat. Vestibulum sed magna at nunc commodo placerat. Praesent blandit. Nam nulla.""},
{""Lorem"":""Sed sagittis. Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus.""},
{""Lorem"":""Curabitur in libero ut massa volutpat convallis. Morbi odio odio, elementum eu, interdum eu, tincidunt in, leo. Maecenas pulvinar lobortis est. Phasellus sit amet erat. Nulla tempus. Vivamus in felis eu sapien cursus vestibulum. Proin eu mi. Nulla ac enim. In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem.""},
{""Lorem"":""Nulla ac enim. In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit. Donec diam neque, vestibulum eget, vulputate ut, ultrices vel, augue.""},
{""Lorem"":""Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris. Morbi non lectus. Aliquam sit amet diam in magna bibendum imperdiet.""},
{""Lorem"":""Nulla ac enim. In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit. Donec diam neque, vestibulum eget, vulputate ut, ultrices vel, augue. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi.""},
{""Lorem"":""Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris. Morbi non lectus. Aliquam sit amet diam in magna bibendum imperdiet. Nullam orci pede, venenatis non, sodales sed, tincidunt eu, felis.""},
{""Lorem"":""Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi. Integer ac neque.""},
{""Lorem"":""In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit. Donec diam neque, vestibulum eget, vulputate ut, ultrices vel, augue. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi. Integer ac neque.""},
{""Lorem"":""Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo. Pellentesque viverra pede ac diam. Cras pellentesque volutpat dui. Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc.""},
{""Lorem"":""Praesent blandit lacinia erat. Vestibulum sed magna at nunc commodo placerat.""},
{""Lorem"":""Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio. Curabitur convallis. Duis consequat dui nec nisi volutpat eleifend. Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus.""},
{""Lorem"":""Lorem ipsum dolor sit amet, consectetuer adipiscing elit.""},
{""Lorem"":""Mauris sit amet eros. Suspendisse accumsan tortor quis turpis. Sed ante. Vivamus tortor. Duis mattis egestas metus. Aenean fermentum. Donec ut mauris eget massa tempor convallis. Nulla neque libero, convallis eget, eleifend luctus, ultricies eu, nibh. Quisque id justo sit amet sapien dignissim vestibulum.""},
{""Lorem"":""Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus. Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis. Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci.""},
{""Lorem"":""Pellentesque at nulla. Suspendisse potenti. Cras in purus eu magna vulputate luctus.""},
{""Lorem"":""Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero.""},
{""Lorem"":""Donec semper sapien a libero. Nam dui. Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio.""},
{""Lorem"":""Donec quis orci eget orci vehicula condimentum.""},
{""Lorem"":""Curabitur convallis.""},
{""Lorem"":""In hac habitasse platea dictumst.""},
{""Lorem"":""Duis aliquam convallis nunc.""},
{""Lorem"":""Quisque id justo sit amet sapien dignissim vestibulum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nulla dapibus dolor vel est. Donec odio justo, sollicitudin ut, suscipit a, feugiat et, eros. Vestibulum ac est lacinia nisi venenatis tristique. Fusce congue, diam id ornare imperdiet, sapien urna pretium nisl, ut volutpat sapien arcu sed augue. Aliquam erat volutpat.""},
{""Lorem"":""Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus. Pellentesque at nulla. Suspendisse potenti. Cras in purus eu magna vulputate luctus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus vestibulum sagittis sapien. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Etiam vel augue.""},
{""Lorem"":""Vestibulum quam sapien, varius ut, blandit non, interdum in, ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio.""},
{""Lorem"":""In hac habitasse platea dictumst. Etiam faucibus cursus urna.""},
{""Lorem"":""In hac habitasse platea dictumst. Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante. Nulla justo.""},
{""Lorem"":""Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum.""},
{""Lorem"":""Nullam varius. Nulla facilisi. Cras non velit nec nisi vulputate nonummy. Maecenas tincidunt lacus at velit. Vivamus vel nulla eget eros elementum pellentesque. Quisque porta volutpat erat. Quisque erat eros, viverra eget, congue eget, semper rutrum, nulla. Nunc purus. Phasellus in felis.""},
{""Lorem"":""Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo. Pellentesque viverra pede ac diam. Cras pellentesque volutpat dui. Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam. Suspendisse potenti. Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris.""},
{""Lorem"":""Nullam varius. Nulla facilisi. Cras non velit nec nisi vulputate nonummy.""},
{""Lorem"":""Nullam sit amet turpis elementum ligula vehicula consequat.""},
{""Lorem"":""Integer tincidunt ante vel ipsum. Praesent blandit lacinia erat. Vestibulum sed magna at nunc commodo placerat. Praesent blandit. Nam nulla.""},
{""Lorem"":""Nulla justo. Aliquam quis turpis eget elit sodales scelerisque. Mauris sit amet eros. Suspendisse accumsan tortor quis turpis. Sed ante. Vivamus tortor. Duis mattis egestas metus.""},
{""Lorem"":""Vivamus vel nulla eget eros elementum pellentesque. Quisque porta volutpat erat. Quisque erat eros, viverra eget, congue eget, semper rutrum, nulla.""},
{""Lorem"":""Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus. Nulla suscipit ligula in lacus. Curabitur at ipsum ac tellus semper interdum.""},
{""Lorem"":""Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam. Suspendisse potenti. Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris. Morbi non lectus. Aliquam sit amet diam in magna bibendum imperdiet. Nullam orci pede, venenatis non, sodales sed, tincidunt eu, felis.""},
{""Lorem"":""Donec dapibus. Duis at velit eu est congue elementum. In hac habitasse platea dictumst. Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante. Nulla justo. Aliquam quis turpis eget elit sodales scelerisque.""},
{""Lorem"":""Proin eu mi. Nulla ac enim. In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit.""},
{""Lorem"":""Fusce congue, diam id ornare imperdiet, sapien urna pretium nisl, ut volutpat sapien arcu sed augue. Aliquam erat volutpat. In congue. Etiam justo. Etiam pretium iaculis justo.""},
{""Lorem"":""Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus. Nulla suscipit ligula in lacus. Curabitur at ipsum ac tellus semper interdum. Mauris ullamcorper purus sit amet nulla.""},
{""Lorem"":""Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin interdum mauris non ligula pellentesque ultrices. Phasellus id sapien in sapien iaculis congue. Vivamus metus arcu, adipiscing molestie, hendrerit at, vulputate vitae, nisl. Aenean lectus. Pellentesque eget nunc. Donec quis orci eget orci vehicula condimentum. Curabitur in libero ut massa volutpat convallis. Morbi odio odio, elementum eu, interdum eu, tincidunt in, leo. Maecenas pulvinar lobortis est.""},
{""Lorem"":""Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus. Nulla suscipit ligula in lacus.""},
{""Lorem"":""Aenean lectus. Pellentesque eget nunc.""},
{""Lorem"":""Morbi sem mauris, laoreet ut, rhoncus aliquet, pulvinar sed, nisl. Nunc rhoncus dui vel sem. Sed sagittis. Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus. Pellentesque at nulla. Suspendisse potenti. Cras in purus eu magna vulputate luctus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus vestibulum sagittis sapien.""},
{""Lorem"":""Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin risus. Praesent lectus. Vestibulum quam sapien, varius ut, blandit non, interdum in, ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio.""},
{""Lorem"":""Cras mi pede, malesuada in, imperdiet et, commodo vulputate, justo. In blandit ultrices enim. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin interdum mauris non ligula pellentesque ultrices. Phasellus id sapien in sapien iaculis congue. Vivamus metus arcu, adipiscing molestie, hendrerit at, vulputate vitae, nisl. Aenean lectus. Pellentesque eget nunc. Donec quis orci eget orci vehicula condimentum. Curabitur in libero ut massa volutpat convallis.""},
{""Lorem"":""Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem. Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst. Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem. Integer tincidunt ante vel ipsum. Praesent blandit lacinia erat. Vestibulum sed magna at nunc commodo placerat. Praesent blandit.""},
{""Lorem"":""Nulla justo. Aliquam quis turpis eget elit sodales scelerisque. Mauris sit amet eros.""},
{""Lorem"":""Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem.""},
{""Lorem"":""Vivamus in felis eu sapien cursus vestibulum. Proin eu mi. Nulla ac enim. In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit.""},
{""Lorem"":""Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio. Curabitur convallis. Duis consequat dui nec nisi volutpat eleifend.""},
{""Lorem"":""In sagittis dui vel nisl. Duis ac nibh. Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus. Suspendisse potenti. In eleifend quam a odio. In hac habitasse platea dictumst. Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem. Quisque ut erat.""},
{""Lorem"":""In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit. Donec diam neque, vestibulum eget, vulputate ut, ultrices vel, augue. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi.""},
{""Lorem"":""Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit.""},
{""Lorem"":""Duis consequat dui nec nisi volutpat eleifend. Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus. Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis. Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum.""},
{""Lorem"":""Aliquam quis turpis eget elit sodales scelerisque. Mauris sit amet eros.""},
{""Lorem"":""Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis. Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh.""},
{""Lorem"":""Integer pede justo, lacinia eget, tincidunt eget, tempus vel, pede. Morbi porttitor lorem id ligula. Suspendisse ornare consequat lectus. In est risus, auctor sed, tristique in, tempus sit amet, sem.""},
{""Lorem"":""In hac habitasse platea dictumst. Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante. Nulla justo. Aliquam quis turpis eget elit sodales scelerisque.""},
{""Lorem"":""Pellentesque ultrices mattis odio. Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus. Nulla suscipit ligula in lacus.""},
{""Lorem"":""Vestibulum quam sapien, varius ut, blandit non, interdum in, ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio. Curabitur convallis.""},
{""Lorem"":""Nulla nisl. Nunc nisl. Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi, in porttitor pede justo eu massa. Donec dapibus. Duis at velit eu est congue elementum. In hac habitasse platea dictumst.""},
{""Lorem"":""Morbi non quam nec dui luctus rutrum. Nulla tellus. In sagittis dui vel nisl. Duis ac nibh. Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus. Suspendisse potenti.""},
{""Lorem"":""In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit. Donec diam neque, vestibulum eget, vulputate ut, ultrices vel, augue. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi. Integer ac neque. Duis bibendum.""}]";

    FirstnameLastnameEmail[] firstnameLastnameEmails;
    int firstnameLastnameLength;
    Sentence[] lorems;
    int loremsLength;

    public SeederServices(Random random)
    {
        this.random = random;
        init();
    }

    public bool RandomBool()
    {
        return random.Next(2) == 1;
    }

    public string Email(double percentNull = .5)
    {
        if (returnNull(percentNull)) return null;

        return firstnameLastnameEmails[random.Next(firstnameLastnameLength)].Email;
    }

    public string Firstname(double percentNull = .5)
    {
        if (returnNull(percentNull)) return null;

        return firstnameLastnameEmails[random.Next(firstnameLastnameLength)].Firstname;
    }

    public DateTime FutureDate(int min, int max)
    {
        int days = random.Next(min, max);
        return DateTime.UtcNow.AddDays(days).AddHours(days).AddMinutes(days);
    }

    public string Lastname(double percentNull = .5)
    {
        if (returnNull(percentNull)) return null;

        return firstnameLastnameEmails[random.Next(firstnameLastnameLength)].Lastname;
    }

    public DateTime PastDate(int min, int max)
    {
        int ago = -random.Next(min, max);
        return DateTime.UtcNow.AddDays(ago).AddHours(ago).AddMinutes(ago);
    }

    public string Phone(double percentNull = .5)
    {
        if (returnNull(percentNull)) return null;

        //courtesy https://www.wikihow.com/Funny-Numbers-to-Call (the other ones on that page don't work or go to real businesses)
        string[] phones = { "2484345508", "7192662837", "8453549912", "5058425662" };

        return phones[random.Next(0, phones.Length)];
    }

    public string RandomString(int min, int max, double percentNull = .5)
    {
        if (returnNull(percentNull)) return null;

        string result = lorems[random.Next(loremsLength)].Lorem;

        while(result.Length < max)
        {
            result += " " + lorems[random.Next(loremsLength)].Lorem;
        }

        int howMany = random.Next(min, max);

        return result.Substring(0, howMany);
    }

    #region helpers

    void init()
    {
        firstnameLastnameEmails = jsonFirstnameLastnameEmail.FromJson<FirstnameLastnameEmail[]>();
        firstnameLastnameLength = firstnameLastnameEmails.Length;

        lorems = jsonSentences.FromJson<Sentence[]>();
        loremsLength = lorems.Length;
    }

    bool returnNull(double percentNull)
    {
        int nullRate = (int)(percentNull * 100);

        if (random.Next(100) < nullRate) return true;
        return false;
    }

    #endregion //helpers
}

public class FirstnameLastnameEmail
{
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}

public class Sentence
{
    public string Lorem { get; set; }
}
