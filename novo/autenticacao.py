import time
from selenium.webdriver.common.by import By


class Autenticacao:
    @staticmethod
    def autenticar_at(driver, empresa):
        driver.get(
            "https://www.acesso.gov.pt/v2/loginForm?partID=PFAP&path=/geral/dashboard"
        )
        time.sleep(0.5)
        driver.find_element(By.XPATH, "//*[@id='radix-:ra:-trigger-N']").click()
        driver.find_element(By.NAME, "username").send_keys(empresa.nif)
        driver.find_element(By.NAME, "password").send_keys(empresa.password_at)
        time.sleep(0.5)
        driver.find_element(
            By.XPATH, "/html/body/div/div/main/div[2]/div[2]/div[1]/div[3]/form/button"
        ).click()

    @staticmethod
    def autenticar_fundos_compensacao(driver, empresa):
        if not empresa.password_ss:
            print(
                f"Empresa {empresa.nome} não tem password SS, não é possível autenticar nos Fundos de Compensação"
            )
            return

        driver.get(
            "https://www.fundoscompensacao.pt/sso/login?service=https%3A%2F%2Fwww.fundoscompensacao.pt%2Ffc%2Fcaslogin"
        )
        time.sleep(0.5)
        driver.find_element(By.ID, "toogleAuth").click()
        time.sleep(1)
        driver.find_element(By.ID, "username").send_keys(empresa.niss)
        time.sleep(0.5)
        driver.find_element(By.ID, "password").send_keys(empresa.password_ss)
        time.sleep(0.5)
        driver.find_element(By.ID, "submitBtn").click()
        time.sleep(0.5)
        continuar_btns = driver.find_elements(By.ID, "continuarBtn")
        if continuar_btns:
            continuar_btns[0].click()

    @staticmethod
    def autenticar_ss(driver, empresa):
        driver.get(
            "https://www.seg-social.pt/sso/login?service=https%3A%2F%2Fwww.seg-social.pt%2Fptss%2Fcaslogin"
        )
        time.sleep(0.5)
        driver.find_element(By.ID, "toogleAuth").click()
        time.sleep(1)
        driver.find_element(By.ID, "username").send_keys(empresa.niss)
        time.sleep(0.5)
        driver.find_element(By.ID, "password").send_keys(empresa.password_ss)
        time.sleep(0.5)
        driver.find_element(By.ID, "submitBtn").click()
        time.sleep(0.5)
        continuar_btns = driver.find_elements(By.ID, "continuarBtn")
        if continuar_btns:
            continuar_btns[0].click()
