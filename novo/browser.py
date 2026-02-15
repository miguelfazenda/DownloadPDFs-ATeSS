from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service

import autenticacao


class Browser:
    driver = None
    drivers_abertos = []

    @staticmethod
    def criar_driver():
        chrome_options = Options()
        chrome_options.add_argument("--disable-search-engine-choice-screen")
        service = Service()
        driver = webdriver.Chrome(service=service, options=chrome_options)
        Browser.drivers_abertos.append(driver)
        return driver

    @staticmethod
    def abre_pedido_certidao(codigo_certidao_permanente):
        driver = Browser.criar_driver()
        driver.get(
            f"https://eportugal.gov.pt/RegistoOnline/Services/CertidaoPermanente/consultaCertidao.aspx?id={codigo_certidao_permanente}"
        )

    @staticmethod
    def abre_portal_das_financas(empresa):
        driver = Browser.criar_driver()
        autenticacao.Autenticacao.autenticar_at(driver, empresa)

    @staticmethod
    def abre_efatura(empresa):
        driver = Browser.criar_driver()
        autenticacao.Autenticacao.autenticar_at(driver, empresa)
        driver.get("https://faturas.portaldasfinancas.gov.pt/")

    @staticmethod
    def abre_seguranca_social(empresa):
        driver = Browser.criar_driver()
        autenticacao.Autenticacao.autenticar_ss(driver, empresa)
        driver.get("https://www.seg-social.pt/ptss/pssd/home?dswid=6586")

    @staticmethod
    def abre_fundos_de_compensacao(empresa):
        driver = Browser.criar_driver()
        autenticacao.Autenticacao.autenticar_fundos_compensacao(driver, empresa)
        driver.get("https://www.fundoscompensacao.pt/fc/gfct/home")

    @staticmethod
    def fecha_drivers_abertos():
        for d in Browser.drivers_abertos:
            d.quit()
        Browser.drivers_abertos.clear()
